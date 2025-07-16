using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using DataBaseInfo.models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http.HttpResults;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using API.Helpers;
using System.Data.Common;

namespace API.Services
{
    public class ProjectService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
       

        public ProjectService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<int> CreateProjectAsync(string projectName)
        {
            try
            {
                

                Project project = new Project(projectName);

                using var context = await _contextFactory.CreateDbContextAsync();

                await context.Projects.AddAsync(project);
                await context.SaveChangesAsync();

                return project.Id;
            }  
            catch (DbUpdateException ex)
            {
                //Логирование ошибки "Error while updateDataBase!"
                throw;
            }
            catch(Exception ex)
            {
                //Логирование ошибки "Unexpected error creating project"
                throw;
            }

            
        }
        public async Task<int> AddUserInProjectAsync(int? userId, int? projectId)
        {
            try
            {
                if (userId == null)
                    throw new ArgumentNullException("Ошибка при получении UserId");
                if (projectId == null)
                    throw new ArgumentNullException("Ошибка при получении ProjectId");
                using var context = _contextFactory.CreateDbContext();
                var user = await context.Users
    .Include(u => u.ProjectUsers) // Явно загружаем ProjectUsers
    .FirstOrDefaultAsync(u => u.Id == userId);

                var project = await context.Projects
                    .Include(p => p.ProjectUsers) // Явно загружаем ProjectUsers
                    .FirstOrDefaultAsync(p => p.Id == projectId);

                var projectUser = new ProjectUser()
                {
                    UserId = (int)userId,
                    ProjectId = (int)projectId,
                    projectRole = "Owner"
                };
                if (user == null || project == null)
                    throw new ArgumentNullException("Произошла ошибка во время связи моделей с ProjectUsers");

                user.ProjectUsers.Add(projectUser);
                project.ProjectUsers.Add(projectUser);
                await context.SaveChangesAsync();
                return projectUser.Id;
            }
            catch (DbUpdateException ex)
            {
                //Логирование ошибки DbUpdateException
                throw;
            }
            catch (ArgumentNullException)
            {
                throw;
                //Ошибка  при получении UserId
            }
            catch (DbException ex)
            {
                throw;
                //Общая ошибка бд
            }
            catch(Exception ex)
            {
                throw;
                //Непредвиденная ошибка
            }
            

        }
    }
}
