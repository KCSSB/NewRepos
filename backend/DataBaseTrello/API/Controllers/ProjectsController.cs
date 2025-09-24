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
        private readonly IRolesHelper _rolesHelper;
        private ErrorContextCreator? _errorContextCreator;
        public ProjectsController(IProjectService projectService,
            IImageService imageService,
            ILogger<ProjectsController> logger, 
            IErrorContextCreatorFactory errCreatorFactory,
            IQueries query,
            IRolesHelper rolesHelper)
        {
            _errCreatorFactory = errCreatorFactory;
            _projectService = projectService;
            _imageService = imageService;
            _logger = logger;
            _query = query;
            _rolesHelper = rolesHelper;
        }
private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(ProjectsController));

        [HttpPost ("CreateProject")]
        public async Task<IActionResult> CreateProject([FromForm] CreateProjectRequest projectRequest)
        {
            if (projectRequest == null)
                return BadRequest();
            if (!ModelState.IsValid)
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
         
            var response = ToResponseMapper.ToHomeProject(query);
            return Ok(response);
        
           
            }
        [HttpDelete("{projectId}/DeleteProjectUsers")]
        public async Task<IActionResult> DeleteProjectUsers(int projectId,[FromBody] DeleteProjectUsersRequest deleteProjectUserRequest)
        {
            if (deleteProjectUserRequest == null || deleteProjectUserRequest.ProjectUsers.Count<=0)
                return BadRequest();
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwner(userId,projectId);
            var projectUsers = deleteProjectUserRequest.ProjectUsers;
            await _projectService.DeleteProjectUsersAsync(projectUsers);
            
            return Ok("Выбранные пользователи успешно исключены из проекта");
        }
        [HttpPatch("{projectId}/UpdateProjectName")]
        public async Task<IActionResult> UpdateProjectName(int projectId,[FromBody] UpdateProjectNameRequest updateProjectNameRequest)
        {
            if (updateProjectNameRequest == null)
                return BadRequest();
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwner(userId , projectId);

            var projectName = updateProjectNameRequest.UpdatedProjectName;

            var updatedProjectName = await _projectService.UpdateProjectNameAsync(projectId, projectName);
            return Ok(new
            {
                Name = updatedProjectName
            });
        }
        [HttpPatch("{projectId}/UpdateProjectDescription")]
        public async Task<IActionResult> UpdateProjectDescription(int projectId, [FromBody] UpdateProjectDescriptonRequest updateProjectDescriptionRequest)
        {
            if (updateProjectDescriptionRequest == null)
                return BadRequest();
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwner(userId, projectId);
            var projectDescription = updateProjectDescriptionRequest.ProjectDescription;
            var updatedDescription = await _projectService.UpdateProjectDescriptionAsync(projectId, projectDescription);
            return Ok(new
            {
                Description = updatedDescription
            });
        }

    }
}
