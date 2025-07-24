using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class GroupController : ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        public GroupController(ILogger<ProjectsController> logger)
        {
            _logger = logger;
        }

    }
}