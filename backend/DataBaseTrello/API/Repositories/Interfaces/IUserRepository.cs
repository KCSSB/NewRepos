using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetDbUserAsync(string email);
        public Task<User?> GetDbUserAsync(int userId);
        public Task AddDbUserAsync(User user);
        public Task<User> ConnectProjectUserAsync(ProjectUser projectUser);
        public Task<User> AddSessionAsync(Session session);
    }
}
