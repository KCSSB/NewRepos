using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface IBoardRepository
    {
        public Task AddDbBoard(Board board);
    }
}
