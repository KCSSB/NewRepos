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
    }
}
