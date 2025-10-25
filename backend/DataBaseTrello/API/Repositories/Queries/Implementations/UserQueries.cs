using API.Repositories.Queries.Intefaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Queries.Implementations
{
    public class UserQueries: IUserQueries
    {
        private readonly AppDbContext _context;
        public UserQueries(AppDbContext context)
        {
            _context = context;
        }
        public async Task<User?> GetUserWithProjectUsersAsync(int userId)
        {
            return await _context.Users
                    .Include(u => u.ProjectUsers)
                    .FirstOrDefaultAsync(u => u.Id == userId);
        }
        
    }
}
