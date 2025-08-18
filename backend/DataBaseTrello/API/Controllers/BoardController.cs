using API.DTO.Requests;
using API.Services;
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
        private readonly BoardService _boardService;
        public BoardController(ILogger<ProjectsController> logger, BoardService boardService) 
        {
           _logger = logger;
            _boardService = boardService;
        }
        [HttpPost("CreateBoard")]
        public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest createBoardRequest)
        {
            Guid boardId = await _boardService.CreateBoardAsync(createBoardRequest.BoardName);

            List<Guid> membersOfBoardId = await _boardService.AddProjectUsersInBoardAsync(boardId, createBoardRequest.BoardLeadId, createBoardRequest.BoardMembers);

           return Ok(new
            {
                BoardId = boardId,
                MembersOfBoardId = membersOfBoardId,
                BoardLeadId = createBoardRequest.BoardLeadId
            });
           
        }
    }
}
