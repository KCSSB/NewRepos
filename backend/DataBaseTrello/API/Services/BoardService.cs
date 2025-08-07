using System.Net;
using API.Constants;
using API.Exceptions.ErrorContext;
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
        public async Task<Guid> CreateBoardAsync(string boardName)
        {
          
            using var context = await _contextFactory.CreateDbContextAsync();

            Board board = new Board
            {
                Name = boardName,
                
            };

            await context.Boards.AddAsync(board);
            await context.SaveChangesWithContextAsync(ServiceName.BoardService,
                OperationName.CreateBoardAsync,"Произошла ошибка при попытке сохранить board в бд",
                UserExceptionMessages.InternalExceptionMessage,
                HttpStatusCode.InternalServerError);

            return board.Id;
        }
        public async Task<Guid> AddProjectUserInBoardAsync(Guid boardId,Guid projectUserId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var board = await context.Boards.Include(b => b.MemberOfBoards)
                .FirstOrDefaultAsync(b => b.Id == boardId);
            
            var projectUser = await context.ProjectUsers.Include(pu => pu.MembersOfBoards)
                .FirstOrDefaultAsync(pu => pu.Id == projectUserId);

            if (projectUser == null)
                throw new AppException(new ErrorContext(ServiceName.BoardService,
                     OperationName.AddProjectUserInBoardAsync,
                     HttpStatusCode.NotFound,
                    UserExceptionMessages.InternalExceptionMessage,
                    $"ProjectUser с Id: {projectUserId}, не найден"));
            if (board == null)
                throw new AppException(new ErrorContext(ServiceName.BoardService,
                 OperationName.AddProjectUserInBoardAsync,
                 HttpStatusCode.NotFound,
                UserExceptionMessages.InternalExceptionMessage,
                $"Board с Id: {boardId}, не найден"));

            MemberOfBoard member = new MemberOfBoard
            {
                BoardId = boardId,
                ProjectUserId = projectUserId,
                BoardRole = (board.MemberOfBoards.Count() == 0) ? "BoardLead" : "BoardMember"
            };

            await context.MembersOfBoards.AddAsync(member);

            await context.SaveChangesWithContextAsync(ServiceName.BoardService,
                OperationName.AddUserInBoardAsync, "Произошла ошибка при попытке сохранить MemberOfGroup в бд",
                UserExceptionMessages.InternalExceptionMessage,
                HttpStatusCode.InternalServerError);

            return member.Id;
        }


    }
}
