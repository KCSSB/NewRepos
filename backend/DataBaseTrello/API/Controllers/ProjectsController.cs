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
using API.Extensions;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly GroupService _groupService;
        
        public ProjectsController(ProjectService projectService, GroupService groupService)
        {
            _projectService = projectService;
          
            _groupService = groupService;
          
        }

        [HttpPost ("CreateProject")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest projectRequest)
        {
        

            Guid userId = User.GetUserIdAsGuidOrThrow();

            Guid projectId = await _projectService.CreateProjectAsync(projectRequest.ProjectName);
            
            Guid projectUserId = await _projectService.AddUserInProjectAsync(userId, projectId);

            
            Guid groupId = await _groupService.CreateGlobalGroupAsync(projectUserId);
            
           Guid memberOfGroupId = await _groupService.AddUserInGroupAsync(projectUserId, groupId);
            return Ok("Проект успешно создан");
        }
        [HttpGet("GetFullProject/{id}")]
        public async Task<IActionResult> GetFullProject(Guid id)
        {
            return Ok();
        }
        
    }
}
