using API.Repositories.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Implementations
{
    public class SessionRepository: ISessionRepository
    {
        private readonly AppDbContext _context;
        public SessionRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Session?>?> GetRangeSessionsAsync(int userId, string deviceId, string token)
        {
            return await _context.Sessions
                .Where(s => s.UserId == userId 
                && s.DeviceId == Guid.Parse(deviceId))
                .ToListAsync();
        }
        public async Task<Session?> GetDbSessionAsync(int userId, string deviceId, string token)
        {
            return await _context.Sessions.FirstOrDefaultAsync(s => token == s.Token
            && s.UserId == userId
            && s.DeviceId == Guid.Parse(deviceId));
        }
        public async Task AddDbSessionAsync(Session session)
        {
            await _context.Sessions.AddAsync(session);
        }
        public void RemoveRangeSessionsAsync(List<Session> sessions)
        {
            _context.Sessions.RemoveRange(sessions);
        }
    }
}
