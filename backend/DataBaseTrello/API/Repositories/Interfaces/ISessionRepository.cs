using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface ISessionRepository
    {
        public Task<List<Session>> GetDbSessions(int userId, string deviceId);
        public Task AddDbSession(ISession session);
        public Task RemoveRangeSessions(List<ISession> session);
    }
}
