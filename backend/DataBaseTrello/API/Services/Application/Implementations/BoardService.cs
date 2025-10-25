﻿using System.Collections.Specialized;
using API.Constants.Roles;
using API.DTO.Mappers;
using API.DTO.Requests.Change;
using API.DTO.Responses.Pages.WorkSpacePage;
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
        public async Task<int> CreateBoardAsync(string boardName, int projectId)
        {
           var project = await _unitOfWork.ProjectRepository.GetProjectAsync(projectId);
            if (project == null)
                throw new AppException(_errCreator.NotFound("Проект не был найден"));

            Board board = new Board
            {
                Name = boardName,
                ProjectId = projectId,
            };
            project.Boards.Add(board);
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
            int count = 0;
            foreach (var boardId in boardIds)
            {
                var board = await _unitOfWork.BoardRepository.GetAsync(boardId);
                if(board!= null)
                {
                    _unitOfWork.BoardRepository.RemoveBoard(board);
                    count++;
                }
            }
            if (count <= 0)
                throw new AppException(_errCreator.NotFound("Доски для удаления не найдены"));
            await _unitOfWork.SaveChangesAsync("Ошибка при удалении досок",ServiceName);
        }
        public async Task UpdateBoardsNameAsync(List<UpdatedBoard> updateBoards)
        {
            int count = 0;
            foreach(var updateBoard in updateBoards)
            {
                var boardId = updateBoard.BoardId;
                var updatedName = updateBoard.UpdatedName;
                var board = await _unitOfWork.BoardRepository.GetAsync(boardId);
                if(board!= null)
                {
                    board.Name = updatedName;
                    count++;
                }
            }
            if (count <= 0)
                throw new AppException(_errCreator.NotFound("Данные для обновления не найдены"));
            await _unitOfWork.SaveChangesAsync("Ошибка при обнолвении названий досок",ServiceName);
    
        }
        public async Task UpdateBoardNameAsync(int boardId, string name)
        {
            var board = await _unitOfWork.BoardRepository.GetAsync(boardId);
            if (board == null)
                throw new AppException(_errCreator.NotFound("Доска не найдена"));

                board.Name = name;

            await _unitOfWork.SaveChangesAsync("Ошибка при обнолвении названия доски", ServiceName);
        }
        public async Task DeleteMembersAsync(int boardId,List<int> membersIds)
        {
            var board = _query.BoardQueries.GetBoardWithMembersAsync(boardId);
            foreach(var memberId in membersIds)
            {
            var member = await _unitOfWork.MembersOfBoardRepository.GetMemberOfBoardAsync(memberId);
                if(member != null)
                    _unitOfWork.MembersOfBoardRepository.RemoveMember(member);
            }
        }
        public async Task<WorkSpaceMember?> AddMemberAsync(int boardId, int projectUserId)
        {
            var board = await _unitOfWork.BoardRepository.GetAsync(boardId);
            if(board==null)
                throw new AppException(_errCreator.NotFound($"Board не найден"));
            var projectUser = await _unitOfWork.ProjectUserRepository.GetProjectUser(projectUserId);
            if (projectUser == null)
                throw new AppException(_errCreator.NotFound($"ProjectUser не найден"));
            var member = new MemberOfBoard
            {
                BoardId = boardId,
                ProjectUserId = projectUserId,
                BoardRole = BoardRoles.Member,
            };
            board.MemberOfBoards.Add(member);
            projectUser.MembersOfBoards.Add(member);
            await _unitOfWork.SaveChangesAsync("Ошибка при добавлении участника доски", ServiceName);
            var workSpaceMember = ToResponseMapper.ToWorkSpaceMember(projectUser, boardId);
            return workSpaceMember;
        }
        
    }
}
