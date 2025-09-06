using API.Repositories.Queries.Intefaces;
using DataBaseInfo;
using DataBaseInfo.models;

namespace API.Repositories.Queries.Implementations
{
    public class BoardQueries: IBoardQueries
    {
        private readonly AppDbContext _context;
        public BoardQueries(AppDbContext context)
        {
            _context = context;
        }
        public Task<List<Board>> GetBoardWithMembers(int boardId)
        {

        }
    }
}
