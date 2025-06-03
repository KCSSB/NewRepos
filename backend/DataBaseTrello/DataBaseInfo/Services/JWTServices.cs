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
    public class JWTServices(IOptions<AuthSettings> options, IDbContextFactory<AppDbContext> contextFactory)
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
        
        public async Task CreateRefreshTokenAsync(User user)
        {
            using(var context = contextFactory.CreateDbContext())
            {
             
                var token = new RefreshToken
                {
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.Add(options.Value.RefreshTokenExpires),
                    IsRevoked = false,
                    UserId = user.Id,
                    

                };
                user.RefreshToken = token;
                await context.RefreshTokens.AddAsync(token);
                

                await context.SaveChangesAsync();
            }
        }
    }
}
