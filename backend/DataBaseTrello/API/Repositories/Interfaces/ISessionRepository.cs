using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface ISessionRepository
    {
        public Task<List<Session>> GetDbSessionsAsync(int userId, string deviceId);
        public Task AddDbSessionAsync(Session session);
        public Task RemoveRangeSessionsAsync(List<Session> session);
    }
}
