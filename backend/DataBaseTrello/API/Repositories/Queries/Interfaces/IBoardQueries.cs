using DataBaseInfo.models;

namespace API.Repositories.Queries.Intefaces
{
    public interface IBoardQueries
    {
        public Task<Board?> GetBoardWithMembersAsync(int boardId);
    }
}
