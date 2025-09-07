using API.Repositories.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;

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
    }
}
