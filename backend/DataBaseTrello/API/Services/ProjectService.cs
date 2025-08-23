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
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                    ProjectName = projectName
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
                    ProjectRole = (project.ProjectUsers.Count <=0) ? "ProjectOwner": "ProjectMember"
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

        public async Task UpdateProjectImageAsync(Guid projectId, string imageUrl)
        {

            using var context = await _contextFactory.CreateDbContextAsync();
            var project = await context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
                throw new AppException(new ErrorContext(ServiceName.ProjectService,
                    OperationName.UpdateProjectImageAsync,
                    HttpStatusCode.InternalServerError,
                    UserExceptionMessages.InternalExceptionMessage,
                    $"Произошла ошибка при обновлении изображения проекта, проект: {projectId}, не найден"));
            project.Avatar = imageUrl;
            await context.SaveChangesWithContextAsync(ServiceName.ProjectService,
                OperationName.UpdateProjectImageAsync,
                $"Произошла ошибка во время обновления изображения проекта {projectId}, Не удалось сохранить изменения в бд",
                UserExceptionMessages.InternalExceptionMessage,
                HttpStatusCode.InternalServerError);

        }
    }
}
