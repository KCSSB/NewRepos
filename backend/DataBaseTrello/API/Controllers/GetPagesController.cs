using API.Extensions;
using API.Services.Application.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

            int userId = User.GetUserId();
            var page = await _getPagesService.CreateHomePageDTOAsync(userId);
            return Ok(page);
        }
        [HttpGet("GetSettingsPage")]
        public async Task<IActionResult> GetSettingsPage()
        {
            int userId = User.GetUserId();
            var page = await _getPagesService.CreateSettingsPageDTOAsync(userId);
            return Ok(page);
        }
    }
}
