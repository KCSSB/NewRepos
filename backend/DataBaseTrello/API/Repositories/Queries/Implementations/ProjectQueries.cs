using API.Repositories.Queries.Intefaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Queries.Implementations
{
    public class ProjectQueries: IProjectQueries
    {
        private readonly AppDbContext _context;
        public ProjectQueries(AppDbContext context)
        {
         _context = context;   
        }
        public async Task<Project?> GetProjectWithUsersAsync(int projectId)
        {
        return await _context.Projects.AsNoTracking().Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User).FirstOrDefaultAsync(p => p.Id == projectId);
        }
        public async Task<Project?> GetProjectWithProjectUsersAsync(int projectId)
        {
        return await _context.Projects
                    .Include(p => p.ProjectUsers) 
                    .FirstOrDefaultAsync(p => p.Id == projectId);
        }
        public async Task<List<Project?>?> GetAllProjectsWhereUserAsync(int userId)
        {
            return await _context.Projects
                .Where(p => p.ProjectUsers.Any(u => u.UserId == userId))
                .Include(p => p.ProjectUsers).ThenInclude(pu => pu.User)
                .ToListAsync();
        }
    }
}
