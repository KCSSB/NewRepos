using DataBaseInfo.models;

namespace API.Repositories.Queries.Intefaces
{
    public interface IBoardQueries
    {
        public Task<List<Board>> GetBoardWithMembers(int boardId);
    }
}
