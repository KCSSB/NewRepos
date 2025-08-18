using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetPagesController : ControllerBase
    {
        private readonly GetPagesService _getPagesService;
        public GetPagesController(GetPagesService getPagesService)
        {
            _getPagesService = getPagesService;
        }
        [HttpGet("GetHomePage")]
        public async Task<IActionResult> GetHomePage()
        {
            Guid userId = User.GetUserId();
            var page = await _getPagesService.GetHomePageAsync(userId);
            return Ok(page);
        }
    }
}
