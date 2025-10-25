using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetDbUserAsync(string email);
        public Task<User?> GetDbUserAsync(int userId);
        public Task AddDbUserAsync(User user);
    }
}
