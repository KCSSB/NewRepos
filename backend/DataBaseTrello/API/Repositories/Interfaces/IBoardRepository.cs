using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface IBoardRepository
    {
        public Task AddAsync(Board board);
        public Task<Board?> GetAsync(int boardId);
        public void RemoveBoard(Board board);
    }
}
