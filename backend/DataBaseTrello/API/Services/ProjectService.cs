using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using DataBaseInfo.models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http.HttpResults;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class ProjectService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ProjectService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async void CreateProject(string? projectName)
        {
            if (projectName.IsNullOrEmpty())
                throw new Exception("Ошибка именования проекта");
            Project project = new Project(projectName);
            using (var context = _contextFactory.CreateDbContext())
            {
                await context.Projects.AddAsync(project);
                await context.SaveChangesAsync();
            }

            
        }
        public async void AddUserInProject(User? user, Project? project)
        {

        }
    }
}
