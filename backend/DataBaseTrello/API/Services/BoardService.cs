using System.Linq;
using System.Net;
using API.Constants;
using API.Exceptions;
using API.Exceptions.Context;
using API.Extensions;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class BoardService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
           private readonly ILogger<BoardService> _logger;
        private readonly ErrorContextCreator _errCreator;
        public BoardService(IDbContextFactory<AppDbContext> contextFactory, ILogger<BoardService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _errCreator = new ErrorContextCreator(ServiceName.BoardService);
        }
        public async Task<int> CreateBoardAsync(string boardName)
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
        public async Task<List<int>> AddProjectUsersInBoardAsync(int boardId, int boardLeadId,List<int> projectUserIds)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var existingBoard = await context.Boards.Include(b => b.MemberOfBoards)
                .FirstOrDefaultAsync(b => b.Id == boardId);
            
            var existingProjectUsers = await context.ProjectUsers
                .Where(pu => projectUserIds.Contains(pu.Id))
                .Include(pu => pu.MembersOfBoards)
                .ToListAsync();

           //Ты хотел создать метод для проверки и синхонизации входных ProjectUsers с теми что хранятся в бд

            if (existingBoard == null)
                throw new AppException(new ErrorContext(ServiceName.BoardService,
                 OperationName.AddProjectUserInBoardAsync,
                 HttpStatusCode.NotFound,
                UserExceptionMessages.InternalExceptionMessage,
                $"Board с Id: {boardId}, не найден"));

            //if (!existingIds.Contains(boardLeadId))
               // throw new Exception();

            var members = CreateBoardMembers(existingProjectUsers, boardId);
            await AddBoardMembersToDbAsync(members);

            //return existingIds;
            return null;
        }
        private async Task VerifyProjectUsersAsync(List<MemberOfBoard> members)
        {

        }

        private List<MemberOfBoard> CreateBoardMembers(List<ProjectUser> projectUsers, int boardId)
        {
           var boardMembers = new List<MemberOfBoard>();
            foreach (var user in projectUsers)
            {

                MemberOfBoard member = new MemberOfBoard
                {
                    BoardId = boardId,
                    ProjectUserId = user.Id,
                    BoardRole = "BoardMember"
                };
                boardMembers.Add(member);
            }
            return boardMembers;
        }
        
        private async Task AddBoardMembersToDbAsync(List<MemberOfBoard> members)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            foreach (var member in members)
            {
                await context.MembersOfBoards.AddAsync(member);
            }
            await context.SaveChangesWithContextAsync(ServiceName.BoardService,
                OperationName.AddBoardMembersToDbAsync,
                "Произошла ошибка при попытке сохранить MembersOfBoard в бд",
                UserExceptionMessages.InternalExceptionMessage,
                HttpStatusCode.InternalServerError);

        }
        
    }
}
