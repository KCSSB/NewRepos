using System.Net.WebSockets;
using API.DTO.Requests;
using API.Extensions;
using API.Services.Application.Interfaces;
using API.Services.Helpers.Interfaces;
using DataBaseInfo.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BoardController: ControllerBase
    {
        private readonly IBoardService _boardService;
        public BoardController(ILogger<ProjectsController> logger, IBoardService boardService) 
        {
            _boardService = boardService;
        }
        [HttpPost("CreateBoard")]
        public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest createBoardRequest)
        {
            var userId = User.GetUserId();
            var projectId = createBoardRequest.ProjectId;
            int boardId = await _boardService.CreateBoardAsync(createBoardRequest.BoardName);
                await _boardService.AddLeadToBoardAsync(boardId, userId,projectId);
            return Ok(new
            {
                BoardId = boardId,
            });
           
        }
    }
}
