using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        public UserController(ILogger<ProjectsController> logger)
        {
            _logger = logger;
        }

    }
}