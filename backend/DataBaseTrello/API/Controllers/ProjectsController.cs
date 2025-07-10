using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using DataBaseInfo.models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ProjectsController(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        [HttpPost ("CreateProject")]
        public async Task<IActionResult> CreateProject()
        {

            return Ok();
        }
        
    }
}
