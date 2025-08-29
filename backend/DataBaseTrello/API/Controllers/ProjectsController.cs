using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using API.Helpers;
using API.Services;
using Microsoft.EntityFrameworkCore;
using API.DTO.Requests;
using API.Exceptions.Context;
using System.Net;
using API.Constants;
using API.Extensions;
using DataBaseInfo;
using API.DTO.Responses;
using DataBaseInfo.models;

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
        
        public ProjectsController(ProjectService projectService, ImageService imageService, ILogger<ProjectsController> logger, ResponseCreator responseCreator)
        {
            _projectService = projectService;
            _imageService = imageService;
            _logger = logger;
            _responseCreator = responseCreator;
           
        }

        [HttpPost ("CreateProject")]
        public async Task<IActionResult> CreateProject([FromForm] CreateProjectRequest projectRequest)
        {
            _logger.LogInformation("Начало создания проекта");
            if(ModelState.IsValid)
            {

            int userId = User.GetUserId();

            int projectId = await _projectService.CreateProjectAsync(projectRequest.ProjectName);
            int projectUserId = await _projectService.AddUserInProjectAsync(userId, projectId);

            var url = DefaultImages.ProjectAvatar;

            if (projectRequest.image!= null && projectRequest.image.Length!=0)
            {
                _logger.LogInformation("Start Update ProjectImage");
                //var image = await _imageService.PrepareImageAsync(projectRequest.image, 1280, 720);
                var result = await _imageService.UploadImageAsync(projectRequest.image, CloudPathes.ProjectImagesPath);
                url = result.url;
            }
                await _projectService.UpdateProjectImageAsync(projectId, url);
            var response = await _responseCreator.CreateSummaryProjectResponseAsync(projectId);
            return Ok(response);
        }
            else
            {
                var errorMessages = string.Join(Environment.NewLine, ModelState.Values
         .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage));
                throw new AppException(new ErrorContext(ServiceName.ProjectsController,
                    OperationName.CreateProject,
                    HttpStatusCode.BadRequest,
                    "Вы указали некорректное изображение",
                    $"Произошли ошибки валидации изображения: \n" + errorMessages));
            }
            }
        [HttpGet("GetFullProject/{id}")]
        public async Task<IActionResult> GetFullProject(int id)
        {

            return Ok();
        }
        
    }
}
