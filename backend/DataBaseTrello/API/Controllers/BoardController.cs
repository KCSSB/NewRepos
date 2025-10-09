using System.Net.WebSockets;
using API.Attributes;
using API.DTO.Mappers;
using API.DTO.Requests;
using API.DTO.Responses.Pages.HallPage;
using API.DTO.Responses.Pages.WorkSpacePage;
using API.Extensions;
using API.Services.Application.Interfaces;
using API.Services.Helpers.Interfaces;
using DataBaseInfo.models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/project/{projectId}/[controller]")]
    [ApiController]
    [Authorize]
    public class BoardController: ControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly IRolesHelper _rolesHelper;
        public BoardController(ILogger<ProjectsController> logger, IBoardService boardService, IRolesHelper rolesHelper)
        {
            _boardService = boardService;
            _rolesHelper = rolesHelper;
        }
        [HttpPost("CreateBoard")]
        public async Task<IActionResult> CreateBoard(int projectId,[FromBody] CreateBoardRequest request)
        {
            if (request== null)
                return BadRequest();
            var userId = User.GetUserId();

            await _rolesHelper.IsProjectOwner(userId, projectId);
            
           int boardId = await _boardService.CreateBoardAsync(request.BoardName, projectId);
           var board = await _boardService.AddLeadToBoardAsync(boardId, userId,projectId);

            var hallBoard = ToResponseMapper.ToHallBoard(board);

            return Ok(hallBoard);
           
        }
        [HttpDelete("DeleteBoards")]
        public async Task<IActionResult> DeleteBoards(int projectId, [FromBody] DeleteBoardsRequest request)
        {
            if (request == null || request.BoardIds.Count<=0)
                return BadRequest();
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwner(userId, projectId);

            var boardIds = request.BoardIds;
            await _boardService.DeleteBoardsAsync(boardIds);
            return Ok("Доски были успешно удалены");
        }
        [HttpPatch("UpdateBoardsName")]
        public async Task<IActionResult> UpdateBoardsName(int projectId, [FromBody] UpdateBoardsNameRequest request)
        {
            if (request == null || request.UpdatedBoards.Count<=0)
                return BadRequest();
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwner(userId, projectId);
            var boardsForUpdate = request.UpdatedBoards;
            await _boardService.UpdateBoardsNameAsync(boardsForUpdate);
            return Ok("названия успешно обновлены");
        }
        [HttpPatch("{boardId}/UpdateBoardName")]
        public async Task<IActionResult> UpdateBoardName(int projectId, int boardId, string name)
        {
            int userId = User.GetUserId();
            await _rolesHelper.IsProjectOwner(userId, projectId);
            //await _boardService.UpdateBoardNameAsync(boardId,name);
            return Ok("Название успешно обновлено");
        }
        [HttpDelete("{boardId}/DeleteMembers")]
        public async Task<IActionResult> DeleteMembers(int projectId, int boardId, [FromBody] DeleteMembersRequest request)
        {
            var userId = User.GetUserId();
            return Ok();
        }
        [HttpPost("{boardId}/AddMember{memberId}")]
        public async Task<IActionResult> AddMember(int projectId, int boardId, int memberId)
        {
            var userId = User.GetUserId();
            var member = new WorkSpaceMember();
            return Ok();
        }
    }
}
