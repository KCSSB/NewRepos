using System.Net.WebSockets;
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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BoardController: ControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly IProjectService _projectService;
        public BoardController(ILogger<ProjectsController> logger, IBoardService boardService, IProjectService projectService) 
        {
            _boardService = boardService;
            _projectService = projectService;
        }
        [HttpPost("CreateBoard")]
        public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest createBoardRequest)
        {
            var userId = User.GetUserId();
            var projectId = createBoardRequest.ProjectId;

             await _projectService.IsProjectOwner(userId, projectId);
            
            int boardId = await _boardService.CreateBoardAsync(createBoardRequest.BoardName);
            var board = await _boardService.AddLeadToBoardAsync(boardId, userId,projectId);

            var hallBoard = ToResponseMapper.ToHallBoard(board);

            return Ok(hallBoard);
           
        }
    }
}
