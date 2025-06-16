using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseInfo.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;

namespace DataBaseInfo.Services
{
    public class UserService:IUserService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly JWTServices _JWTService;
        public UserService(IDbContextFactory<AppDbContext> contextFactory,JWTServices JWTService)
        {
            _contextFactory = contextFactory;
            _JWTService = JWTService;
        }
        public User Register(string userEmail, string password)
        {
            using(var context = _contextFactory.CreateDbContext())
            {
                User? em = context.Users.FirstOrDefault(em => em.UserEmail == userEmail);
                if (em != null)
                {
                    throw new Exception("Пользователь с таким Email уже существует");
                }

            var user = new User
            {
                UserEmail = userEmail,
                UserName = "Temporary_Name",
            };
            var passHash = new PasswordHasher<User>().HashPassword(user, password);
                if(passHash == null)
                {
                    throw new Exception("Ошибка при хэшировании пароля");
                }
            user.UserPassword = passHash;
            return user;
                }
                
                
            }
        
        public async Task<(string AcessToken,string? RefreshToken)> Login(string UserEmail, string Password)
        {
         
            
            using (var context = _contextFactory.CreateDbContext())
            {
                User? user = await context.Users.FirstOrDefaultAsync(u => u.UserEmail == UserEmail);
                if(user == null)
                {
                    throw new Exception("Пользователь не найден");
                }
                var activeToken = await context.RefreshTokens
        .Include(rt => rt.User)
        .FirstOrDefaultAsync(rt => rt.User.UserEmail == UserEmail
                               && !rt.IsRevoked
                               && rt.ExpiresAt > DateTime.UtcNow);
                if(activeToken != null)
                {
                    throw new Exception("Вы уже были авторизованы");
                }
                
                var result = new PasswordHasher<User?>().VerifyHashedPassword(user, user.UserPassword, Password);
                if (result == PasswordVerificationResult.Success)
                {
                    var token = Guid.NewGuid().ToString();
                    await _JWTService.CreateRefreshTokenAsync(user, token);
                    return (_JWTService.GenerateAcessToken(user), token);
                }
                else
                {
                    throw new Exception("inccorrect password");
                }
            
            
            }
        }
      
        // Получение пользователя по ID
       /* public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        // Создание нового пользователя
        public async Task<bool> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }
        // Обновление данных пользователя по ID
        public async Task<bool> UpdateUserAsync(int id, User updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return false;

            existingUser.UserName = updatedUser.UserName;
            existingUser.UserPassword = updatedUser.UserPassword;
            // Обновите другие необходимые поля

            _context.Users.Update(existingUser);
            return await _context.SaveChangesAsync() > 0;
        }

        // Удаление пользователя по ID
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }*/
    }
}
