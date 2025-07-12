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

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly TokenExtractorService _tokenExtractor;
        public ProjectsController(ProjectService projectService, TokenExtractorService tokenExtractor)
        {
            _projectService = projectService;
            _tokenExtractor = tokenExtractor;
        }

        [HttpPost ("CreateProject")]
        public async Task<IActionResult> CreateProject(string? projectName)
        {
            if (projectName.IsNullOrEmpty())
                return BadRequest("Ошибка именования проекта");

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            if (accessToken.IsNullOrEmpty())
                return BadRequest("Ошибка при получении acess токена");

            int userId = _tokenExtractor.TokenExtractorId(accessToken);
            int? projectId = await _projectService.CreateProjectAsync(accessToken, projectName);
            if(projectId==null)
                return BadRequest("Project creation failed");
            bool success = await _projectService.AddUserInProjectAsync(userId, (int)projectId);
            if (!success)
                return BadRequest("Ошибка во время привязки User к Project");

            return Ok("Проект успешно создан");

        }
        
    }
}
