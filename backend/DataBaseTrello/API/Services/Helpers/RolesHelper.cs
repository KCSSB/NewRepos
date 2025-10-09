using API.Constants.Roles;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Repositories.Uof;
using API.Services.Helpers.Implementations;
using API.Services.Helpers.Interfaces;
using DataBaseInfo.models;

namespace API.Services.Helpers
{
    public class RolesHelper: IRolesHelper
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(EmailService));
        public RolesHelper(IUnitOfWork unitOfWork, IErrorContextCreatorFactory errCreatorFactory)
        {
            _errCreatorFactory = errCreatorFactory;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> IsProjectOwner(int userId, int projectId)
        {
            var projectUser = await _unitOfWork.ProjectUserRepository.GetProjectUser(userId, projectId);
            if (projectUser == null || projectUser.ProjectRole != ProjectRoles.Owner)
                throw new AppException(_errCreator.Forbidden($"Пользователь не является основателем проекта"));
            return true;
        }
        public async Task<bool> IsLeadOfBoard(int userId, int boardId)
        {
            var memberOfBoard = await _unitOfWork.MembersOfBoardRepository.GetMemberOfBoardAsync(userId, boardId);
            if (memberOfBoard == null || memberOfBoard.BoardRole != BoardRoles.Leader)
                throw new AppException(_errCreator.Forbidden($"Пользователь не является главой доски"));
            return true;
        }
        public async Task<bool> IsProjectOwnerOrLeaderOfBoard(int userId, int projectId, int boardId)
        {
            try
            {
                bool isOwner = await IsProjectOwner(userId, projectId);
                return isOwner;
            }
            catch (AppException)
            {
                try
                {
                bool isLead = await IsLeadOfBoard(userId, boardId);
                    return isLead;
                }
                catch (AppException)
                {
                    throw;
                }
                
            }
        }
    }
}
