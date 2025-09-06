using API.Repositories.Queries.Intefaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Queries.Implementations
{
    public class SessionQueries: ISessionQueries
    {
        private readonly AppDbContext _context;
        public SessionQueries(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Session>?>? GetActiveSessions(int userId)
        {
            return await _context.Sessions
                .Where(s => s.UserId == userId
                && !s.IsRevoked
                && s.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
        }
    }
}
