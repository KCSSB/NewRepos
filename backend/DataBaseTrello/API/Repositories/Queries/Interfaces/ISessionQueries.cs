using DataBaseInfo.models;

namespace API.Repositories.Queries.Intefaces
{
    public interface ISessionQueries
    {
        public Task<List<Session?>?> GetActiveSessions(int userId);
        public Task<Session?> GetCurSessionWithUser(string token);
    }
}
