using API.Repositories.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;

namespace API.Repositories.Implementations
{
    public class SessionRepository: ISessionRepository
    {
        private readonly AppDbContext _context;
        public SessionRepository(AppDbContext context)
        {
            _context = context;
        }
        }
        public Task<List<Session>> GetDbSessionsAsync(int userId, string deviceId)
        {

        }
        public Task AddDbSessionAsync(Session session)
        {

        }
        public Task RemoveRangeSessionsAsync(List<Session> session)
        {

        }
    }
}
