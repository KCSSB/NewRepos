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
            
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            int userId = _tokenExtractor.TokenExtractorId(accessToken);
            int projectId = await _projectService.CreateProjectAsync(projectRequest.ProjectName);

            int projectUserId = await _projectService.AddUserInProjectAsync(userId, projectId);

            //Остановился
            int groupId = await _groupService.CreateGroupAsync(projectUserId);
            if (groupId == null)
                return BadRequest("Ошибка при создании группы");
            int? memberOfGroupId = await _groupService.AddUserInGroupAsync((int)projectUserId, (int)groupId);
            if (memberOfGroupId == null)
                return BadRequest("Ошибка при добавлении основателя в группу");
            return Ok("Проект успешно создан");
            
            


        }
        
    }
}
