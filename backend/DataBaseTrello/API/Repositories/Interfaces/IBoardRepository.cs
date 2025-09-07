using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface IBoardRepository
    {
        public Task AddAsync(Board board);
    }
}
