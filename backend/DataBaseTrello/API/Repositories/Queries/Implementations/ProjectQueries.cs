using API.DTO.Mappers;
using API.DTO.Responses.Pages.HallPage;
using API.Repositories.Queries.Intefaces;
using DataBaseInfo;
using DataBaseInfo.Entities;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Queries.Implementations
{
    public class ProjectQueries : IProjectQueries
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
        public async Task<Project?> GetProjectForHallAsync(int userId,int projectId)
        {
            var project = await _context.Projects
                .Where(p => p.Id == projectId)
                .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.User)
                .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.MembersOfBoards)
                        .ThenInclude(mob => mob.Board)
                            .ThenInclude(b => b.Cards)
                                .ThenInclude(c => c.Tasks)
                                    .ThenInclude(t => t.SubTasks)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            if (project == null)
                return null;

            return project;
        }
    }
}
