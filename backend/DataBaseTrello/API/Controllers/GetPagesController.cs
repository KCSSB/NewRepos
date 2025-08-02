using API.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetPagesController : ControllerBase
    {
        [HttpGet]
        public Task<IActionResult> GetHomePage()
        {
            Guid userId = User.GetUserIdAsGuidOrThrow();

        }
    }
}
