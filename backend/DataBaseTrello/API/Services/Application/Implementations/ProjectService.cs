using DataBaseInfo.models;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using API.Extensions;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Services.Application.Interfaces;
using API.Repositories.Queries.Intefaces;
using API.Repositories.Uof;
using API.Repositories.Queries;
using API.Constants.Roles;

namespace API.Services.Application.Implementations
{
    public class ProjectService: IProjectService
    {
        private readonly string ServiceName = nameof(ProjectService);
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueries _query;

        public ProjectService(IErrorContextCreatorFactory errCreatorFactory,
            IUnitOfWork unitOfWork,
            IQueries query)
        {
            _errCreatorFactory = errCreatorFactory;
            _unitOfWork = unitOfWork;
            _query = query;
        }
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(ServiceName);
        public async Task<int> CreateProjectAsync(string projectName)
        {
                Project project = new Project
                {
                    ProjectName = projectName
                };


            await _unitOfWork.ProjectRepository.AddAsync(project);

            await _unitOfWork.SaveChangesAsync(ServiceName, "Произошла ошибка, в момент добавления проекта в базу данных");

                return project.Id;

        }
        public async Task<int> AddUserInProjectAsync(int userId, int projectId)
        {

            var user = await _query.UserQueries.GetUserWithProjectUsersAsync(userId);

            var project = await _query.ProjectQueries.GetProjectWithProjectUsersAsync(projectId);


            var projectUser = new ProjectUser()
            {
                UserId = userId,
                ProjectId = projectId,
                ProjectRole = project.ProjectUsers.Count <= 0 ? ProjectRoles.Owner : ProjectRoles.Member
            };
                
                if (user == null)
                    throw new AppException(_errCreator.NotFound($"Произошла ошибка в момент добавления пользователя в проект, Пользователь id: {userId}, не найден"));

                if (project == null)
                    throw new AppException(_errCreator.NotFound($"Произошла ошибка в момент добавления пользователя в проект, Проект id: {projectId}, не найден"));

                user.ProjectUsers.Add(projectUser);

                project.ProjectUsers.Add(projectUser);

            await _unitOfWork.SaveChangesAsync(ServiceName, $"Ошибка в момент добавления пользователя Id: {userId} в проект Id: {projectId}");

                return projectUser.Id;  
        }

        public async Task UpdateProjectImageAsync(int projectId, string imageUrl)
        {

            var project = await _unitOfWork.ProjectRepository.GetProjectAsync(projectId);
            if (project == null)
                throw new AppException(_errCreator.NotFound($"Произошла ошибка при обновлении изображения проекта, проект: {projectId}, не найден"));
            project.Avatar = imageUrl;
            await _unitOfWork.SaveChangesAsync(ServiceName, $"Произошла ошибка во время обновления изображения проекта {projectId}, Не удалось сохранить изменения в бд");

        }
        public async Task DeleteProjectUsersAsync(List<int> projectUsersIds)
        {
            foreach (var projectUserId in projectUsersIds)
            {
                var projectUser = await _unitOfWork.ProjectUserRepository.GetProjectUser(projectUserId);
                if (projectUser != null && projectUser.ProjectRole!=ProjectRoles.Owner)
                {
                   _unitOfWork.ProjectUserRepository.RemoveProjectUser(projectUser);
                }
            }
            await _unitOfWork.SaveChangesAsync("Произошла ошибка во время исключения участников проекта", ServiceName);
        }
        public async Task<string> UpdateProjectNameAsync(int projectId, string projectName)
        {
            var project = await _unitOfWork.ProjectRepository.GetProjectAsync(projectId);
            project.ProjectName = projectName;
            await _unitOfWork.SaveChangesAsync("Ошибка при изменении названия проекта" , ServiceName);
            return project.ProjectName;
        }
    }
}
