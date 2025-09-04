using System.Linq;
using System.Net;
using API.Constants;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Extensions;
using API.Middleware;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services.Application.Implementations
{
    public class BoardService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogger<BoardService> _logger;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;


        public BoardService(IDbContextFactory<AppDbContext> contextFactory, ILogger<BoardService> logger, IErrorContextCreatorFactory errCreatorFactory)
        {
        _errCreatorFactory = errCreatorFactory;
            _contextFactory = contextFactory;
            _logger = logger;
        }
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(BoardService));
        public async Task<int> CreateBoardAsync(string boardName)
        {
          
            using var context = await _contextFactory.CreateDbContextAsync();

            Board board = new Board
            {
                Name = boardName,
            };

            await context.Boards.AddAsync(board);
            await context.SaveChangesWithContextAsync("Произошла ошибка при попытке сохранить board в бд");

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
                throw new AppException(_errCreator.NotFound($"Board с Id: {boardId}, не найден"));

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
            await context.SaveChangesWithContextAsync("Произошла ошибка при попытке сохранить MembersOfBoard в бд");

        }
        
    }
}
