using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseInfo.models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using API.Helpers;
namespace DataBaseInfo.Services
{
    public class JWTServices(IOptions<AuthSettings> options, IDbContextFactory<AppDbContext> contextFactory, HashService hash)
    {
        public string GenerateAcessToken(User user)
        {
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
        
        public async Task CreateRefreshTokenAsync(User user, string token)
        {
            using(var context = contextFactory.CreateDbContext())
            {
                
                var hashedToken = new RefreshToken
                {
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.Add(options.Value.RefreshTokenExpires),
                    Token = hash.HashToken(token),
                    IsRevoked = false,
                    UserId = user.Id,
                    User = user
                };
                user.RefreshToken = hashedToken;
                await context.RefreshTokens.AddAsync(hashedToken);
                
                
                await context.SaveChangesAsync();
               
            }
        }
        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken)
        {
            using(var context = contextFactory.CreateDbContext())
            {

            var storedToken = await context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => hash.VerifyToken(refreshToken, rt.Token));

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            storedToken.IsRevoked = true;
            await context.SaveChangesAsync();

            var newRefreshToken = new RefreshToken
            {
                Token = hash.HashToken(Guid.NewGuid().ToString()),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(options.Value.RefreshTokenExpires),
                UserId = storedToken.UserId,
                User = storedToken.User
            };

                storedToken.User.RefreshToken = newRefreshToken;

                await context.RefreshTokens.AddAsync(newRefreshToken);
            
            await context.SaveChangesAsync();

            var newAccessToken = GenerateAcessToken(storedToken.User);

            return (newAccessToken, newRefreshToken.Token);
            }
        }
    }
}
