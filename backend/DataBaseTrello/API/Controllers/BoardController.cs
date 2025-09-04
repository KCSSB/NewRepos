using API.DTO.Requests;
using API.Services.Application.Interfaces;
using API.Services.Helpers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BoardController: ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        private readonly IBoardService _boardService;
        public BoardController(ILogger<ProjectsController> logger, IBoardService boardService) 
        {
           _logger = logger;
            _boardService = boardService;
        }
        [HttpPost("CreateBoard")]
        public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest createBoardRequest)
        {
            int boardId = await _boardService.CreateBoardAsync(createBoardRequest.BoardName);

            List<int> membersOfBoardId = await _boardService.AddProjectUsersInBoardAsync(boardId, createBoardRequest.BoardLeadId, createBoardRequest.BoardMembers);

           return Ok(new
            {
                BoardId = boardId,
                MembersOfBoardId = membersOfBoardId,
                BoardLeadId = createBoardRequest.BoardLeadId
            });
           
        }
    }
}
