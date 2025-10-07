using API.DTO.Requests;
using API.DTO.Responses.Pages.WorkSpacePage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/project/{projectId}/board/{boardId}/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        public CardController()
        {

        }
        [HttpDelete("DeleteCards")]
        public async Task<IActionResult> DeleteCards(int projectId, int boardId, List<int> CardIds)
        {
            return Ok("Карточки были удалены");
        }
        [HttpPatch("ChangeCardsNames")]
        public async Task<IActionResult> ChangeCardsNames(int projectId, int boardId, [FromBody] ChangeCardNames request)
        {
            return Ok("Названия успешно изменены");
        }
        [HttpPost("CreateCard")]
        public async Task<IActionResult> CreateCard(int projectId, int boardId)
        {
            WorkSpaceCard card = new WorkSpaceCard();
            return Ok(card);
        }
    }
}
