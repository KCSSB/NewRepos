using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseInfo.models;

namespace DataBaseInfo.Services
{
    internal class ProjectService:IProjectService
    {
        private readonly AppDbContext _context;
        public ProjectService(AppDbContext context)
        {
            _context = context;
        }
        // Получение пользователя по ID
        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects.FindAsync(id);
        }
        // Создание нового пользователя
        public async Task<bool> CreateProjectAsync(Project project, int TemporaryUserId, string ProjectName)
        {
            _context.Projects.Add(project);
            project.ProjectName = ProjectName;
            User? user = await _context.Users.FindAsync(TemporaryUserId);
            
            return await _context.SaveChangesAsync() > 0;
        }
        // Обновление данных пользователя по ID
        public async Task<bool> UpdateProjectAsync(int id, Project updatedProject)
        {
            var existingProject = await _context.Projects.FindAsync(id);
            if (existingProject == null)
                return false;

            existingProject.ProjectName = updatedProject.ProjectName;
         
            // Обновите другие необходимые поля

            _context.Projects.Update(existingProject);
            return await _context.SaveChangesAsync() > 0;
        }

        // Удаление пользователя по ID
        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return false;

            _context.Projects.Remove(project);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

