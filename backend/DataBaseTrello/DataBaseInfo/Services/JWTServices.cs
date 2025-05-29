using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseInfo.models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
namespace DataBaseInfo.Services
{
    public class JWTServices(IOptions<AuthSettings> options)
    {
        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("UserName", user.UserName),
                new Claim("UserEmail", user.UserEmail),
                new Claim("UserId", user.Id.ToString()),
            };
            var jwtToken = new JwtSecurityToken(
                expires: DateTime.UtcNow.Add(options.Value.expires),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
