using API.Repositories.Queries.Intefaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Queries.Implementations
{
    public class BoardQueries: IBoardQueries
    {
        private readonly AppDbContext _context;
        public BoardQueries(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Board?> GetBoardWithMembersAsync(int boardId)
        {
            return await _context.Boards.Include(b => b.MemberOfBoards)
                .FirstOrDefaultAsync(b => b.Id == boardId);
        }
        public async Task<Board?> GetWorkSpaceAsync(int boardId)
        {
            return await _context.Boards
        .Where(b => b.Id == boardId)
    .Include(b => b.Cards).ThenInclude(c => c.Tasks)
        .ThenInclude(t => t.SubTasks)
    .Include(b => b.MemberOfBoards)
        .ThenInclude(m => m.ProjectUser).ThenInclude(pu => pu.Project)
    .FirstOrDefaultAsync();
        }
    }
}
