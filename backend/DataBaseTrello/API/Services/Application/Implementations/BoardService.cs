using API.Constants.Roles;
using API.DTO.Requests;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Extensions;
using API.Repositories.Interfaces;
using API.Repositories.Queries;
using API.Repositories.Queries.Intefaces;
using API.Repositories.Queries.Interfaces;
using API.Repositories.Uof;
using API.Services.Application.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Application.Implementations
{
    public class BoardService: IBoardService
    {
        private readonly string ServiceName = nameof(BoardService);
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private readonly ILogger<IBoardService> _logger;
        private ErrorContextCreator? _errorContextCreator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueries _query;


        public BoardService(ILogger<IBoardService> logger, 
            IErrorContextCreatorFactory errCreatorFactory, 
            IUnitOfWork unitOfWork,
            IQueries query)
        {
            _errCreatorFactory = errCreatorFactory;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _query = query;
        }
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(IBoardService));
        public async Task<int> CreateBoardAsync(string boardName)
        {
            Board board = new Board
            {
                Name = boardName,
            };
            await _unitOfWork.BoardRepository.AddAsync(board);
            await _unitOfWork.SaveChangesAsync(ServiceName, "Произошла ошибка при попытке сохранить board в бд");
            return board.Id;
        }
        public async Task<List<int>> AddProjectUsersInBoardAsync(int boardId,List<int> projectUserIds = null)
        {

            var existingBoard = await _query.BoardQueries.GetBoardWithMembersAsync(boardId);
            
            var existingProjectUsers = await _query.ProjectUserQueries.GetProjectUsersWithMembersByIdsAsync(projectUserIds);

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
        public async Task<Board?> AddLeadToBoardAsync(int boardId, int userId, int projectId)
        {
            var lead = await _unitOfWork.ProjectUserRepository.GetProjectUser(userId,projectId);

            var leadOfBoard = CreateBoardLeader(boardId, lead);
            var board = await _query.BoardQueries.GetBoardWithMembersAsync(boardId);
            board.LeadOfBoardId = lead.Id;
            board.MemberOfBoards.Add(leadOfBoard);
            await _unitOfWork.SaveChangesAsync("Ошибка во время добавления лидера доски", ServiceName);
            return board;
        }
        private MemberOfBoard CreateBoardLeader(int boardId, ProjectUser projectUser)
        {
            return new MemberOfBoard
            {
                BoardId = boardId,
                ProjectUserId = projectUser.Id,
                BoardRole = BoardRoles.Leader,
            };
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
                    BoardRole = "Member"
                };
                boardMembers.Add(member);
            }
            return boardMembers;
        }
        
        private async Task AddBoardMembersToDbAsync(List<MemberOfBoard> members)
        {
            await _unitOfWork.MembersOfBoardRepository.AddMemberRangeAsync(members);

            await _unitOfWork.SaveChangesAsync(ServiceName,"Произошла ошибка при попытке сохранить MembersOfBoard в бд");
        }

        public async Task DeleteBoardsAsync(List<int> boardIds)
        {
            foreach (var boardId in boardIds)
            {
                var board = await _unitOfWork.BoardRepository.GetAsync(boardId);
                if(board!= null)
                {
                    _unitOfWork.BoardRepository.RemoveBoard(board);
                }
            }
            await _unitOfWork.SaveChangesAsync("Ошибка при удалении досок",ServiceName);
        }
        public async Task UpdateBoardsNameAsync(List<UpdatedBoard> updateBoards)
        {
            foreach(var updateBoard in updateBoards)
            {
                var boardId = updateBoard.BoardId;
                var updatedName = updateBoard.UpdatedName;
                var board = await _unitOfWork.BoardRepository.GetAsync(boardId);
                if(board!= null)
                {
                    board.Name = updatedName;
                }
            }
            await _unitOfWork.SaveChangesAsync("Ошибка при обнолвении названий досок",ServiceName);
    
        }
    }
}
