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
using API.Requests;

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
        public ProjectsController(ProjectService projectService, TokenExtractorService tokenExtractor, GroupService groupService)
        {
            _projectService = projectService;
            _tokenExtractor = tokenExtractor;
            _groupService = groupService;
        }

        [HttpPost ("CreateProject")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest projectRequest)
        {
            if (projectRequest.ProjectName.IsNullOrEmpty())
                return BadRequest("Ошибка именования проекта");

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            if (accessToken.IsNullOrEmpty())
                return BadRequest("Ошибка при получении acess токена");

            int userId = _tokenExtractor.TokenExtractorId(accessToken);
            int? projectId = await _projectService.CreateProjectAsync(projectRequest.ProjectName);

            if(projectId==null)
                return BadRequest("Ошибка во время создания проекта");
            int? projectUserId = await _projectService.AddUserInProjectAsync(userId, (int)projectId);

            if (projectUserId==null)
                return BadRequest("Ошибка во время привязки User к Project");
            int? groupId = await _groupService.CreateGroupAsync((int)projectUserId);
            if (groupId == null)
                return BadRequest("Ошибка при создании группы");
            int? memberOfGroupId = await _groupService.AddUserInGroupAsync((int)projectUserId, (int)groupId);
            if (memberOfGroupId == null)
                return BadRequest("Ошибка при добавлении основателя в группу");
            return Ok("Проект успешно создан");

        }
        
    }
}
