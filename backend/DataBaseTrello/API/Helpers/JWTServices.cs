using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBaseInfo.models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using API.Configuration;
using System.Runtime.CompilerServices;
using API.Extensions;
using API.Exceptions;
using API.Exceptions.ErrorContext;
using API.Exceptions.Enumes;
using System.Net;
namespace API.Helpers
{
    public class JWTServices(IOptions<AuthSettings> options, IDbContextFactory<AppDbContext> contextFactory, HashService hash)
    {
        public string GenerateAccessToken(User? user)
        {
            try
            {
                if (user == null)
                    throw new Exception("Ошибка при получении пользователя");
                }
        catch (Exception)
                {
                //Логирование ошибки при получении пользователя
                throw;
                }
            var claims = new List<Claim>
            {
               new Claim("UserName", user.UserName),
                new Claim("UserEmail", user.UserEmail),
                new Claim("UserId", user.Id.ToString()),
            };
            var jwtToken = new JwtSecurityToken(
                expires: DateTime.UtcNow.Add(options.Value.AccessTokenExpires),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        

    

        public async Task CreateRefreshTokenAsync(User? user, string token)
        {
            try
            {
                if (user == null)
                    throw new Exception("Ошибка при получении пользователя");
                using (var context = contextFactory.CreateDbContext())
                {

                    var hashedToken = new RefreshToken
                    {
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.Add(options.Value.RefreshTokenExpires),
                        Token = hash.HashToken(token),
                        IsRevoked = false,
                        UserId = user.Id,

                    };
                    user.RefreshToken = hashedToken;
                    await context.RefreshTokens.AddAsync(hashedToken);


                    await context.SaveChangesAsync();

                }
            }
            catch (DbUpdateException ex)
            {
                //Логгирование ошибки DbUpdateException
                throw;
            }
            catch (Exception ex)
            {
                    //Логгриование ошибки При получении пользователя
                    throw;
                
            }
        }
        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken)
        {
            
               
            using(var context = contextFactory.CreateDbContext())
            {
                var HashToken = hash.HashToken(refreshToken);
            var storedToken = await context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => HashToken == rt.Token);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
               throw new AppException(new ErrorContext(ServiceName.JWTServices,
                   OperationName.RefreshTokenAsync,
                   HttpStatusCode.Unauthorized,
                   "Ошибка авторизации",
                   "RefreshToken не существует в базе данных или его время истекло"));

            storedToken.IsRevoked = true;
                await context.SaveChangesWithContextAsync(ServiceName.JWTServices,
                    OperationName.RefreshTokenAsync,
                    "Ошибка при отзыве старого Refresh token",
                    "Не удалось завершить авторизацию. Повторите попытку позже",
                    HttpStatusCode.InternalServerError);
            //Возможны проблемы
                var token = Guid.NewGuid().ToString();
            var newRefreshToken = new RefreshToken
            {
                Token = hash.HashToken(token),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(options.Value.RefreshTokenExpires),
                UserId = storedToken.UserId,
               
            };

                storedToken.User.RefreshToken = newRefreshToken;

                await context.RefreshTokens.AddAsync(newRefreshToken);
            //Возможны проблемы
            await context.SaveChangesAsync();
            //Возможны проблемы
            //Возможны проблемы
            var newAccessToken = GenerateAccessToken(newRefreshToken.User);
            //Возможны проблемы
            return (newAccessToken, token);
            }
            
            
    
        }
        public async Task<bool> RevokeRefreshTokenAsync(string? refreshToken) // Где то тут может быть ошибка, поищи
        {
            try
            {
                if (string.IsNullOrEmpty(refreshToken))
                    throw new Exception("Рефреш токен отсутствует");
                using (var context = contextFactory.CreateDbContext())
                {

                    var hashedRequestToken = hash.HashToken(refreshToken);
                    var token = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == hashedRequestToken);
                    if (token == null)
                        throw new Exception("Ошибка при получении Рефреш токена из базы данных");

                    context.RefreshTokens.Remove(token);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                //Рефреш токен отсутствует в бд
                throw;
            }
        }
    }
}
