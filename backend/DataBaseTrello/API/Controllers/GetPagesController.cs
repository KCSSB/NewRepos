using API.DTO.Responses.Pages.HallPage;
using API.Extensions;
using API.Services.Application.Interfaces;
using API.Services.Helpers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GetPagesController : ControllerBase
    {
        private readonly IGetPagesService _getPagesService;
        public GetPagesController(IGetPagesService getPagesService)
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
        [HttpGet("GetHallPage/{id}")]
        public async Task<IActionResult> GetHallPage(int id)
        {
            var userId = User.GetUserId();
            
            HallPage? hallPage = await _getPagesService.CreateHallPageDTOAsync(userId, id);
            return Ok(hallPage);
        }
    }
}
