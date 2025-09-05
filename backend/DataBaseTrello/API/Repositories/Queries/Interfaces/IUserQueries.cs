using DataBaseInfo.models;

namespace API.Repositories.Queries.Intefaces
{
    public interface IUserQueries
    {
        public Task<User?> GetUserWithProjectUsers(int userId);
    }
}
