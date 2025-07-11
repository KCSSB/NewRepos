using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Helpers;
using DataBaseInfo.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;

namespace DataBaseInfo.Services
{
    public class UserService
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
                    throw new Exception("Пользователь с таким Email уже существует"); //Выходит ошибка, но никак не обрабатывается
                }

            var user = new User
            {
                UserEmail = userEmail,
                UserName = "Temporary_Name",
            };
            var passHash = new PasswordHasher<User>().HashPassword(user, password);
                if(passHash == null)
                {
                    throw new Exception("Ошибка при хэшировании пароля"); //Выходит ошибка но не обрабатывается
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
                    throw new Exception("Пользователь не найден"); //Выходит ошибка но не обрабатывается
                }
                var activeToken = await context.RefreshTokens
        .Include(rt => rt.User)
        .FirstOrDefaultAsync(rt => rt.User.UserEmail == UserEmail
                               && !rt.IsRevoked
                               && rt.ExpiresAt > DateTime.UtcNow);
                if(activeToken != null)
                {
                    throw new Exception("Вы уже были авторизованы"); //Выходит ошибка но не обрабатывается
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
                    throw new Exception("inccorrect password"); //Выходит ошибка но не обрабатывается
                }
            
            
            }
        }
      
        
    }
}
