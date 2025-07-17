using System;
using System.Collections.Generic;
using System.Data.Common;
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
        public UserService(IDbContextFactory<AppDbContext> contextFactory, JWTServices JWTService)
        {
            _contextFactory = contextFactory;
            _JWTService = JWTService;
        }
        public async Task<User?> RegisterAsync(string userEmail, string password)
        {
            try
            {

                using (var context = _contextFactory.CreateDbContext())
                {
                    User? em = await context.Users.FirstOrDefaultAsync(em => em.UserEmail == userEmail);
                    if (em != null)
                        throw new InvalidOperationException("Пользователь с таким Email уже существует"); //Ошибка существования идентичного Email


                    var user = new User
                    {
                        UserEmail = userEmail,
                        UserName = "Temporary_Name",
                    };
                    var passHash = new PasswordHasher<User>().HashPassword(user, password);
                    if (passHash == null)
                        throw new Exception("Ошибка при хэшировании пароля"); //Ошибка при хэшировании пароля

                    user.UserPassword = passHash;
                    return user;

                }
            }
            catch (InvalidCastException ex) //Ошибка при хэшировании пароля
            {
                //Логирование ошибки
                throw;
            }
            catch (Exception ex) //Ошибка существования идентичного Email
            {
                //Логирование ошибки
                throw;
            }
            }


        

        public async Task<(string? AccessToken, string? RefreshToken)?> LoginAsync(string UserEmail, string Password)
        {

            try
            {

                using (var context = _contextFactory.CreateDbContext())
                {
                    User? user = await context.Users.FirstOrDefaultAsync(u => u.UserEmail == UserEmail);
                    if (user == null)
                        throw new Exception("Пользователь не найден"); //Ошибка ненахождения пользователя
                    
                    var activeToken = await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.User.UserEmail == UserEmail
                                   && !rt.IsRevoked
                                   && rt.ExpiresAt > DateTime.UtcNow);
                    if (activeToken != null)
                        throw new Exception("Вы уже были авторизованы"); //Выходит ошибка но не обрабатывается
                    

                    var result = new PasswordHasher<User?>().VerifyHashedPassword(user, user.UserPassword, Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        var token = Guid.NewGuid().ToString();
                        await _JWTService.CreateRefreshTokenAsync(user, token);
                        return (_JWTService.GenerateAccessToken(user), token);
                    }
                    else
                    {
                        throw new Exception("inccorrect password"); //Выходит ошибка но не обрабатывается
                    }
                }
            }
            catch (Exception)
            {
                throw;
                //Логирование ошибки UpdateDbContext
            }
        }
    } 
        
    
}
