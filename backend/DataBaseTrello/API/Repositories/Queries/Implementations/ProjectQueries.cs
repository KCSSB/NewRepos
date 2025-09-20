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
        public async Task<HallPage?> GetProjectForHall(int userId, int projectId)
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

            // Формируем DTO
            var hallPage = new HallPage
            {
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                Description = project.Description,

                ProjectRole = project.ProjectUsers
                    .FirstOrDefault(pu => pu.UserId == userId)?.ProjectRole,

                ProjectUsers = project.ProjectUsers.Select(pu => new HallProjectUser
                {
                    ProjectUserId = pu.Id,
                    FirstName = pu.User?.FirstName ?? string.Empty,
                    LastName = pu.User?.SecondName ?? string.Empty,
                    ProjectRole = pu.ProjectRole
                }).ToList(),

                Boards = project.ProjectUsers
                    .SelectMany(pu => pu.MembersOfBoards.Select(mob => mob.Board))
                    .Distinct()
                    .Select(b => new HallBoard
                    {
                        BoardId = b.Id,
                        BoardName = b.Name,
                        MembersCount = b.MemberOfBoards.Count,

                        ProgressBar = b.Cards
                            .SelectMany(c => c.Tasks)
                            .SelectMany(t => t.SubTasks)
                            .DefaultIfEmpty()
                            .Count() > 0
                                ? (int)(
                                    (double)b.Cards
                                        .SelectMany(c => c.Tasks)
                                        .SelectMany(t => t.SubTasks)
                                        .Count(st => st.IsCompleted)
                                    / b.Cards
                                        .SelectMany(c => c.Tasks)
                                        .SelectMany(t => t.SubTasks)
                                        .Count() * 100
                                  )
                                : 0,

                        BoardLeadId = b.LeadOfBoardId,

                        DateOfStartWork = b.Cards
                            .SelectMany(c => c.Tasks)
                            .Where(t => t.DateOfStartWork != null)
                            .Select(t => t.DateOfStartWork!.Value)
                            .DefaultIfEmpty(DateOnly.MinValue)
                            .Min(),

                        DateOfDeadline = b.Cards
                            .SelectMany(c => c.Tasks)
                            .Where(t => t.DateOfDeadline != null)
                            .Select(t => t.DateOfDeadline!.Value)
                            .DefaultIfEmpty(DateOnly.MinValue)
                            .Max()
                    }).ToList()
            };

            return hallPage;
        }
    }
}
