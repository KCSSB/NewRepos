using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using DataBaseInfo.models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http.HttpResults;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Services
{
    public class ProjectService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
       

        public ProjectService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<int?> CreateProjectAsync(string accessToken, string projectName)
        {
            try
            {
                

                Project project = new Project(projectName);

                using var context = _contextFactory.CreateDbContext();

                await context.Projects.AddAsync(project);
                await context.SaveChangesAsync();

                return project.Id;
            }  
            catch (DbUpdateException ex)
            {
                //Логирование ошибки "Error while updateDataBase!"
                return null;
            }
            catch(Exception ex)
            {
                //Логирование ошибки "Unexpected error creating project"
                return null;
            }

            
        }
        public async Task<bool> AddUserInProjectAsync(int userId, int projectId)
        {
            try
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
                    projectRole = "Owner"
                };
                if (user == null || project == null)
                    throw new Exception("Произошла ошибка во время связи моделей с ProjectUsers");

                user.ProjectUsers.Add(projectUser);
                project.ProjectUsers.Add(projectUser);
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                //Логирование ошибки DbUpdateException
                return false;
            }
          
        }
    }
}
