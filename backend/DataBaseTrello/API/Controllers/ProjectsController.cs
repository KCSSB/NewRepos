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
using API.Exceptions.ErrorContext;
using System.Net;
using API.Constants;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly TokenExtractorService _tokenExtractor;
        private readonly GroupService _groupService;
        private readonly ILogger<ProjectsController> _logger;
        public ProjectsController(ProjectService projectService, TokenExtractorService tokenExtractor, GroupService groupService, ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _tokenExtractor = tokenExtractor;
            _groupService = groupService;
            _logger = logger;
        }

        [HttpPost ("CreateProject")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest projectRequest)
        {
            _logger.LogInformation(InfoMessages.StartOperation + OperationName.CreateProject);
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if(string.IsNullOrEmpty(accessToken))
                throw new AppException(new ErrorContext(ServiceName.ProjectsController,
                OperationName.CreateProject,
                HttpStatusCode.BadRequest,
                UserExceptionMessages.CreateProjectExceptionMessage,
                "Данные переданные в экземпляр RegisterUserRequest не валидны"));

            int userId = _tokenExtractor.TokenExtractorId(accessToken);
            int projectId = await _projectService.CreateProjectAsync(projectRequest.ProjectName);
            
            int projectUserId = await _projectService.AddUserInProjectAsync(userId, projectId);

            //Остановился
            int groupId = await _groupService.CreateGlobalGroupAsync(projectUserId);
            
            int memberOfGroupId = await _groupService.AddUserInGroupAsync(projectUserId, groupId);
            _logger.LogInformation(InfoMessages.FinishOperation + OperationName.CreateProject);
            return Ok("Проект успешно создан");
        }
        
    }
}
