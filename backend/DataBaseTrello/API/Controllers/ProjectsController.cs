using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Helpers;
using API.Services;
using API.DTO.Requests;
using API.Exceptions.Context;
using API.Constants;
using API.Extensions;
using DataBaseInfo;
using API.Exceptions;


namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly ImageService _imageService;
        private readonly ILogger<ProjectsController> _logger;
        private readonly ResponseCreator _responseCreator;
        private readonly ErrorContextCreator _errCreator;
        
        public ProjectsController(ProjectService projectService, ImageService imageService, ILogger<ProjectsController> logger, ResponseCreator responseCreator)
        {
            _projectService = projectService;
            _imageService = imageService;
            _logger = logger;
            _responseCreator = responseCreator;
            _errCreator = new ErrorContextCreator(ServiceName.ProjectsController);
        }

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
            var response = await _responseCreator.CreateSummaryProjectResponseAsync(projectId);
            return Ok(response);
        
           
            }
        [HttpGet("GetFullProject/{id}")]
        public async Task<IActionResult> GetFullProject(int id)
        {

            return Ok();
        }
        
    }
}
