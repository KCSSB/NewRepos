using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore.Query;

namespace DataBaseInfo.Services
{
    internal class ProjectUserService
    {
       /* private readonly AppDbContext _context;
        ProjectUserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateProjectUserAsync(ProjectUser ProjectUser, User user, Project project)
        {
            _context.ProjectUsers.Add(ProjectUser);
            ProjectUser.User = user;
            ProjectUser.UserId = user.Id;
            ProjectUser.Project = project;
            ProjectUser.ProjectId = project.Id;
            //Место для Group
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<ProjectUser?> GetProjectUserById(int Id)
        {
            return await _context.ProjectUsers.FindAsync(Id);
        }
        public async Task<bool> UpdateProjectUserAsync(int id, ProjectUser updatedProjectUser)
        {
            var existingProjectUser = await _context.ProjectUsers.FindAsync(id);
            if (existingProjectUser == null)
            {
                return false;
            }
            
            existingProjectUser.UserId = updatedProjectUser.UserId;
            existingProjectUser.ProjectId = updatedProjectUser.ProjectId;

            await _context.Entry(existingProjectUser).Reference(pu => pu.User).LoadAsync();
            await _context.Entry(existingProjectUser).Reference(pu => pu.Project).LoadAsync();

            existingProjectUser.Groups.Clear();
            existingProjectUser.LedGroups.Clear();

            foreach (var group in updatedProjectUser.Groups)
            {
                existingProjectUser.Groups.Add(group);
            }
            foreach (var group in updatedProjectUser.LedGroups)
            {
                existingProjectUser.LedGroups.Add(group);
            }

            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<>

        */
    }
}
