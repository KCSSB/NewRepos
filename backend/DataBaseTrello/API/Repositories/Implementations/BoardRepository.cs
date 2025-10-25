using API.Repositories.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class BoardRepository: IBoardRepository
    {
        private readonly AppDbContext _context;
        public BoardRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Board board)
        {
            await _context.Boards.AddAsync(board);
        }
        public async Task<Board?> GetAsync(int boardId)
        {
            return await _context.Boards.FirstOrDefaultAsync(b => b.Id == boardId);
        }
        public void RemoveBoard(Board board)
        {
             _context.Remove(board);
        }
    }
}
