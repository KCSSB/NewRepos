using API.Repositories.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace API.Repositories.Implementations
{
    public class UserRepository: IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<User?> GetDbUserAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserEmail== email);
        }
        public async Task<User?> GetDbUserAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id==userId);
        }
        public async Task AddDbUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }
        
    }
}
