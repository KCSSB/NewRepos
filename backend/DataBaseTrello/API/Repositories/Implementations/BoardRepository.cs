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
        public Task AddDbBoard(Board board)
        {

        }
    }
}
