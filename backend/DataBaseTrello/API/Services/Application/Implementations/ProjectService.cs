using DataBaseInfo.models;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using API.Extensions;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Services.Application.Interfaces;

namespace API.Services.Application.Implementations
{
    public class ProjectService: IProjectService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;

        public ProjectService(IDbContextFactory<AppDbContext> contextFactory, IErrorContextCreatorFactory errCreatorFactory)
        {
        _errCreatorFactory = errCreatorFactory;
            _contextFactory = contextFactory;
          
        }
private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(ProjectService));
        public async Task<int> CreateProjectAsync(string projectName)
        {
                Project project = new Project
                {
                    ProjectName = projectName
                };

                using var context = _contextFactory.CreateDbContext();

                await context.Projects.AddAsync(project);

                await context.SaveChangesWithContextAsync("Произошла ошибка, в момент добавления проекта в базу данных");

                return project.Id;

        }
        public async Task<int> AddUserInProjectAsync(int userId, int projectId)
        {
              
                await using var context = await _contextFactory.CreateDbContextAsync();

                var user = await context.Users
                    .Include(u => u.ProjectUsers) // Явно загружаем ProjectUsers
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var project = await context.Projects
                    .Include(p => p.ProjectUsers) // Явно загружаем ProjectUsers
                    .FirstOrDefaultAsync(p => p.Id == projectId);

          
            var projectUser = new ProjectUser()
                {
                    UserId = userId,
                    ProjectId = projectId,
                    ProjectRole = project.ProjectUsers.Count <=0 ? "ProjectOwner": "ProjectMember"
                };
                
                if (user == null)
                    throw new AppException(_errCreator.NotFound($"Произошла ошибка в момент добавления пользователя в проект, Пользователь id: {userId}, не найден"));

                if (project == null)
                    throw new AppException(_errCreator.NotFound($"Произошла ошибка в момент добавления пользователя в проект, Проект id: {projectId}, не найден"));

                user.ProjectUsers.Add(projectUser);

                project.ProjectUsers.Add(projectUser);
                
                await context.SaveChangesWithContextAsync($"Ошибка в момент добавления пользователя Id: {userId} в проект Id: {projectId}");

                return projectUser.Id;
           
            

        }

        public async Task UpdateProjectImageAsync(int projectId, string imageUrl)
        {

            using var context = await _contextFactory.CreateDbContextAsync();
            var project = await context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
                throw new AppException(_errCreator.NotFound($"Произошла ошибка при обновлении изображения проекта, проект: {projectId}, не найден"));
            project.Avatar = imageUrl;
            await context.SaveChangesWithContextAsync($"Произошла ошибка во время обновления изображения проекта {projectId}, Не удалось сохранить изменения в бд");

        }
    }
}
