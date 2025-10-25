using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface ISessionRepository
    {
        public Task<List<Session>> GetRangeSessionsAsync(int userId, string deviceId, string token);
        public Task AddDbSessionAsync(Session session);
        public void RemoveRangeSessionsAsync(List<Session> session);
        public Task<Session?> GetDbSessionAsync(int userId, string deviceId,string token);
    }
}
