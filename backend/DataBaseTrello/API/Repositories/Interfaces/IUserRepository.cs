using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetDbUser(string email);
        public Task<User?> GetDbUser(int userId);
        public Task AddDbUser(User user);
        public Task<User> ConnectProjectUser(ProjectUser projectUser);
        public Task<User> AddSession(Session session);
    }
}
