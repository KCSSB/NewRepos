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
                if ( em==null) 
                {

            var user = new User
            {
                UserEmail = userEmail,
                UserName = "Temporary_Name",
            };
            var passHash = new PasswordHasher<User>().HashPassword(user, password);
            user.UserPassword = passHash;
            return user;
                }
                else
                {
                    throw new Exception("Пользователь с таким Email уже существует");
                }
                
            }
        }
        public async Task<(string AcessToken,string? RefreshToken)> Login(string UserEmail, string Password)
        {
            User? user;
            using (var context = _contextFactory.CreateDbContext())
            {
                user = context.Users.FirstOrDefault(u => u.UserEmail == UserEmail);
            }
            if (user != null)
            {

                var result = new PasswordHasher<User?>().VerifyHashedPassword(user, user.UserPassword, Password);
                if (result == PasswordVerificationResult.Success)
                {
                    var token = Guid.NewGuid().ToString();
                    await _JWTService.CreateRefreshTokenAsync(user, token);
                    return (_JWTService.GenerateAcessToken(user), token);
                }
                else
                {
                    throw new Exception("Unauthorize");
                }
            }
            else
            {
                throw new Exception("Пользователь не найден");
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
