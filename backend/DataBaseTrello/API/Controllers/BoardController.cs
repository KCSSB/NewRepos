using System.Net.WebSockets;
using API.Attributes;
using API.DTO.Mappers;
using API.DTO.Requests;
using API.DTO.Responses.Pages.HallPage;
using API.Extensions;
using API.Services.Application.Interfaces;
using API.Services.Helpers.Interfaces;
using DataBaseInfo.models;
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
        public async Task<IActionResult> CreateBoard(int projectId,[FromBody] CreateBoardRequest createBoardRequest)
        {
            var userId = User.GetUserId();

            await _rolesHelper.IsProjectOwner(userId, projectId);
            
            int boardId = await _boardService.CreateBoardAsync(createBoardRequest.BoardName);
            var board = await _boardService.AddLeadToBoardAsync(boardId, userId,projectId);

            var hallBoard = ToResponseMapper.ToHallBoard(board);

            return Ok(hallBoard);
           
        }
        [HttpDelete("DeleteBoards")]
        public async Task<IActionResult> DeleteBoards(int projectId, [FromBody] DeleteBoardsRequest deleteBoardsRequest)
        {
            return Ok("Всё удалил насяльника");
        }
        [HttpPatch("UpdateBoardsName")]
        public async Task<IActionResult> UpdateBoardsName(int projectId, [FromBody] UpdateBoardsNameRequest updateBoardsNameRequest)
        {
            return Ok("Всё обновил насяльника");
        }
    }
}
