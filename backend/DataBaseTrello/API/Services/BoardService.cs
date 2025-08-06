using System.Net;
using API.Constants;
using API.Extensions;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class BoardService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
           private readonly ILogger<BoardService> _logger;
        public BoardService(IDbContextFactory<AppDbContext> contextFactory, ILogger<BoardService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }
        public async Task<Guid> CreateBoardAsync(string boardName, Guid LeadOfBoard)
        {
          
            using var context = await _contextFactory.CreateDbContextAsync();

            Board board = new Board
            {
                Name = boardName,
                LeadOfBoardId = LeadOfBoard,
            };

            await context.Boards.AddAsync(board);
            await context.SaveChangesWithContextAsync(ServiceName.BoardService,
                OperationName.CreateBoardAsync,"Произошла ошибка при попытке сохранить board в бд",
                UserExceptionMessages.InternalExceptionMessage,
                HttpStatusCode.InternalServerError);

            return board.Id;
        }
    }
}
