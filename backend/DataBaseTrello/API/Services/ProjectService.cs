using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using DataBaseInfo.models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http.HttpResults;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using API.Helpers;
using System.Data.Common;
using API.Extensions;
using API.Exceptions.ErrorContext;
using System.Net;
using API.Constants;

namespace API.Services
{
    public class ProjectService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
       

        public ProjectService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<Guid> CreateProjectAsync(string projectName)
        {
            
                Project project = new Project
                {
                    ProjectName = projectName,
                    Avatar = DefaultImages.ProjectAvatar
                };

                using var context = await _contextFactory.CreateDbContextAsync();

                await context.Projects.AddAsync(project);

                await context.SaveChangesWithContextAsync(ServiceName.ProjectService,
                    OperationName.CreateProjectAsync,
                    UserExceptionMessages.CreateProjectExceptionMessage,
                    "Произошла ошибка, в момент добавления проекта в базу данных",
                    HttpStatusCode.InternalServerError);

                return project.Id;

        }
        public async Task<Guid> AddUserInProjectAsync(Guid userId, Guid projectId)
        {
              
                using var context = _contextFactory.CreateDbContext();

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
                    projectRole = (project.ProjectUsers.Count <=0) ? "Owner": "Member"
                };
                if (user == null)
                    throw new AppException(new ErrorContext(ServiceName.ProjectService,
                        OperationName.AddUserInProjectAsync,
                        HttpStatusCode.InternalServerError,
                        UserExceptionMessages.InternalExceptionMessage,
                        $"Произошла ошибка в момент добавления пользователя в проект, Пользователь id: {userId}, не найден"));

                if (project == null)
                    throw new AppException(new ErrorContext(ServiceName.ProjectService,
                        OperationName.AddUserInProjectAsync,
                        HttpStatusCode.InternalServerError,
                        UserExceptionMessages.InternalExceptionMessage,
                        $"Произошла ошибка в момент добавления пользователя в проект, Проект id: {projectId}, не найден"));

                user.ProjectUsers.Add(projectUser);

                project.ProjectUsers.Add(projectUser);

                await context.SaveChangesWithContextAsync(ServiceName.ProjectService,
                    OperationName.AddUserInProjectAsync,
                    $"Ошибка в момент добавления пользователя Id: {userId} в проект Id: {projectId}",
                    UserExceptionMessages.CreateProjectExceptionMessage,
                    HttpStatusCode.InternalServerError);

                return projectUser.Id;
           
            

        }
        public async Task<Project> GetFullProjectAsync(Guid Id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var project = await context.Projects.AsNoTracking().Where(p => p.Id==Id)
                .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.Boards)
                .ThenInclude(mb => mb.Board)
                .ThenInclude(b => b.Cards)
                .ThenInclude(c => c.Tasks).FirstOrDefaultAsync();
            return project;
        }
    }
}
