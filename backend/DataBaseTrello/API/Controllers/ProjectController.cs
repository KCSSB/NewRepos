using DataBaseInfo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo.models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   
    public class ProjectController : ControllerBase
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;


        public ProjectController(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }


    }
}
