using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Extensions;
using API.Services.Application.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Application.Implementations
{
    public class BoardService: IBoardService
    {
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private readonly ILogger<IBoardService> _logger;
        private ErrorContextCreator? _errorContextCreator;
        private readonly AppDbContext _context;


        public BoardService(ILogger<IBoardService> logger, IErrorContextCreatorFactory errCreatorFactory, AppDbContext context)
        {
            _errCreatorFactory = errCreatorFactory;
            _logger = logger;
            _context = context;
        }
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(IBoardService));
        public async Task<int> CreateBoardAsync(string boardName)
        {
          
           

            Board board = new Board
            {
                Name = boardName,
            };

            await _context.Boards.AddAsync(board);
            await _context.SaveChangesWithContextAsync("Произошла ошибка при попытке сохранить board в бд");

            return board.Id;
        }
        public async Task<List<int>> AddProjectUsersInBoardAsync(int boardId, int boardLeadId,List<int> projectUserIds)
        {
    

            var existingBoard = await _context.Boards.Include(b => b.MemberOfBoards)
                .FirstOrDefaultAsync(b => b.Id == boardId);
            
            var existingProjectUsers = await _context.ProjectUsers
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
        
            foreach (var member in members)
            {
                await _context.MembersOfBoards.AddAsync(member);
            }
            await _context.SaveChangesWithContextAsync("Произошла ошибка при попытке сохранить MembersOfBoard в бд");

        }
        
    }
}
