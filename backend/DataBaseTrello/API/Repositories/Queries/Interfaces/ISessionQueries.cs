using DataBaseInfo.models;

namespace API.Repositories.Queries.Intefaces
{
    public interface ISessionQueries
    {
        public Task<List<Session?>?> GetExpiredSessions();
        public Task<List<Session?>?> GetActiveSessions();
        public Task<Session?> GetCurrentSessionWithUser(string token);
    }
}
