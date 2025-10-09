using API.DTO.Mappers;
using API.DTO.Requests;
using API.DTO.Responses.Pages.WorkSpacePage;
using API.Extensions;
using API.Services.Application.Implementations;
using API.Services.Helpers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/project/{projectId}/board/{boardId}/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly CardService _cardService;
        private readonly IRolesHelper _rolesHelper;
        public CardController(CardService cardService, IRolesHelper rolesHelper)
        {
            _cardService = cardService;
            _rolesHelper = rolesHelper;
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
            int userId = User.GetUserId();
            await _rolesHelper.IsProjectOwnerOrLeaderOfBoard(userId, projectId, boardId);

            var card = await _cardService.CreateCardAsync(boardId);
            var workSpaceCard = ToResponseMapper.ToWorkSpaceCard(card);
            return Ok(workSpaceCard);
        }
    }
}
