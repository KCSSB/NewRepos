using API.Constants.Roles;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Repositories.Uof;
using API.Services.Helpers.Implementations;
using API.Services.Helpers.Interfaces;

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
        public async Task IsProjectOwner(int userId, int projectId)
        {
            var projectUser = await _unitOfWork.ProjectUserRepository.GetProjectUser(userId, projectId);
            if (projectUser == null || projectUser.ProjectRole != ProjectRoles.Owner)
                throw new AppException(_errCreator.Forbidden($"Пользователь {userId}, не является основателем проекта {projectId}"));
        }
    }
}
