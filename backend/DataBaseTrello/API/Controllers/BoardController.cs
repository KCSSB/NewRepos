using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BoardController: ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        public BoardController(ILogger<ProjectsController> logger) 
        {
           _logger = logger;
    }

    }
}
