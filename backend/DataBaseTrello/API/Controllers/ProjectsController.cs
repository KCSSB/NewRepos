using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProjectsController : ControllerBase
    {
       
        [HttpPost ("CreateProject")]
        public async Task<IActionResult> CreateProject()
        {
            
            var acessToken = await HttpContext.GetTokenAsync("access_token");
            var jwtHandler = new JwtSecurityTokenHandler();
            var token = jwtHandler.ReadJwtToken(acessToken);
            var UserIdClaim = token?.Claims?.FirstOrDefault(c => c.Type == "UserId");
            int? UserId = int.Parse(UserIdClaim.Value);
            return Ok(UserId);
        }
        
    }
}
