using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.DTO.Requests;
using API.Exceptions.Context;
using API.Constants;
using API.Extensions;
using DataBaseInfo;
using API.Middleware;
using API.Exceptions.ContextCreator;
using API.Services.Application.Interfaces;
using API.Services.Helpers.Interfaces;

using Microsoft.EntityFrameworkCore;
using API.DTO.Mappers;
using API.Repositories.Queries.Intefaces;
using API.Repositories.Queries;


namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IImageService _imageService;
        private readonly ILogger<ProjectsController> _logger;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private readonly IQueries _query;
        private ErrorContextCreator? _errorContextCreator;
        public ProjectsController(IProjectService projectService, IImageService imageService, ILogger<ProjectsController> logger, IErrorContextCreatorFactory errCreatorFactory, IQueries query)
        {
            _errCreatorFactory = errCreatorFactory;
            _projectService = projectService;
            _imageService = imageService;
            _logger = logger;
            _query = query;
        }
private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(ProjectsController));

        [HttpPost ("CreateProject")]
        public async Task<IActionResult> CreateProject([FromForm] CreateProjectRequest projectRequest)
        {
            _logger.LogInformation("Начало создания проекта");
            if(!ModelState.IsValid)
            {
                var errorMessages = string.Join(Environment.NewLine, ModelState.Values
         .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage));
            throw new AppException(_errCreator.BadRequest($"Произошли ошибки валидации изображения: \n" + errorMessages));
            }

            int userId = User.GetUserId();

            int projectId = await _projectService.CreateProjectAsync(projectRequest.ProjectName);
            var projectUserId = await _projectService.AddUserInProjectAsync(userId, projectId);

            var url = DefaultImages.ProjectAvatar;

            if (projectRequest.image!= null && projectRequest.image.Length!=0)
            {   
                var result = await _imageService.UploadImageAsync(projectRequest.image, CloudPathes.ProjectImagesPath);
                url = result.url;
            }
           
                await _projectService.UpdateProjectImageAsync(projectId, url);

            var query = await _query.ProjectQueries.GetProjectWithUsersAsync(projectId);
            if (query == null)
                throw new AppException(_errCreator.InternalServerError("Не удалось получить проект из базы данных"));
         
            var response = ToResponseMapper.ToSummaryProjectResponse(query);
            return Ok(response);
        
           
            }
        [HttpGet("GetFullProject/{id}")]
        public async Task<IActionResult> GetFullProject(int id)
        {

            return Ok();
        }
        
    }
}
