using API.Repositories.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class ProjectUserRepository: IProjectUserRepository
    {
        private readonly AppDbContext _context;
        public ProjectUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectUser?> GetProjectUser(int userId, int projectId)
        {
            return await _context.ProjectUsers.FirstOrDefaultAsync(pu => pu.ProjectId==projectId && pu.UserId ==userId);
        }
        public async Task<ProjectUser?> GetProjectUser(int projectUserId)
        {
            return await _context.ProjectUsers.FirstOrDefaultAsync(pu => pu.Id == projectUserId);
        }
        public void RemoveProjectUser(ProjectUser projectUser)
        {
            _context.Remove(projectUser);
        }
    }
}
