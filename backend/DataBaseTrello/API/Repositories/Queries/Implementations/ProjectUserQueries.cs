using API.Repositories.Queries.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Queries.Implementations
{
    public class ProjectUserQueries: IProjectUserQueries
    {
        private readonly AppDbContext _context;
        public ProjectUserQueries(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<ProjectUser?>?> GetProjectUsersWithMembersByIdsAsync(List<int> projectUserIds)
        {

        return await _context.ProjectUsers
                .Where(pu => projectUserIds.Contains(pu.Id))
                .Include(pu => pu.MembersOfBoards)
                .ToListAsync();
    }
        }
}
