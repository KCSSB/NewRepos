using System.Text;
using DataBaseInfo.models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using API.Configuration;
using API.Extensions;
using API.Exceptions.Context;
using System.Net;
using API.Constants;
using API.Exceptions;
namespace API.Helpers
{
    public class JWTServices
    {
        private readonly IOptions<AuthSettings> _options;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly HashService _hashService;
        private readonly ILogger<JWTServices> _logger;
        private readonly ErrorContextCreator _errCreator;
        public JWTServices(IOptions<AuthSettings> options,
            IDbContextFactory<AppDbContext> contextFactory,
            HashService hashService,
            ILogger<JWTServices> logger)
        {
            _contextFactory = contextFactory;
            _hashService = hashService;
            _options = options;
            _logger = logger;
            _errCreator = new ErrorContextCreator(ServiceName.JWTServices);
        }
        public string GenerateAccessToken(User user)
        {
        
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim("FirstName", user.FirstName),
                new Claim("SecondName", user.SecondName), 
               new Claim("Sex", SexHelper.GetSexDisplay(user.Sex)),
               new Claim("InviteId", user.InviteId.ToString()),
                new Claim("UserEmail", user.UserEmail),
                new Claim("Avatar", user.Avatar)
                
            };
            
            var jwtToken = new JwtSecurityToken(
                expires: DateTime.UtcNow.Add(_options.Value.AccessTokenExpires),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey)), SecurityAlgorithms.HmacSha256));
          

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            
            
        }
        

    

        public async Task CreateRefreshTokenAsync(User user, string token)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
                

                    var hashedToken = new RefreshToken
                    {
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.Add(_options.Value.RefreshTokenExpires),
                        Token = _hashService.HashToken(token),
                        IsRevoked = false,
                        UserId = user.Id,
                    };
                    user.RefreshToken = hashedToken;
              
                await context.RefreshTokens.AddAsync(hashedToken);

              
                await context.SaveChangesWithContextAsync("Ошибка при сохранении Hashed Refresh Token");
                
            }
          
        
        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken)
        {


            using var context = _contextFactory.CreateDbContext();
            
                var HashToken = _hashService.HashToken(refreshToken);

            var storedToken = await context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => HashToken == rt.Token);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
               throw new AppException(_errCreator.Unauthorized("RefreshToken не существует в базе данных или его время истекло"));

            storedToken.IsRevoked = true;

            await context.SaveChangesWithContextAsync("Ошибка при отзыве старого Refresh token");
           
            var token = Guid.NewGuid().ToString();
            var newRefreshToken = new RefreshToken
            {
                Token = _hashService.HashToken(token),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(_options.Value.RefreshTokenExpires),
                UserId = storedToken.UserId,
            };

                storedToken.User.RefreshToken = newRefreshToken;

                await context.RefreshTokens.AddAsync(newRefreshToken);
          
            await context.SaveChangesWithContextAsync("Ошибка при сохранении RefreshToken в базу данных");
            
            var newAccessToken = GenerateAccessToken(newRefreshToken.User);
         
            return (newAccessToken, token);
            }
            
            
    
        
        public async Task RevokeRefreshTokenAsync(string refreshToken) // Где то тут может быть ошибка, поищи
        {

            using var context = _contextFactory.CreateDbContext();

            var hashedRequestToken = _hashService.HashToken(refreshToken);
            var token = await context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == hashedRequestToken);
            if (token == null)
                throw new AppException(_errCreator.Unauthorized("В базе не найден refresh token, соответствующий значению из cookie при попытке выйти из аккаунта"));

            context.RefreshTokens.Remove(token);
                    await context.SaveChangesWithContextAsync($"Ошибка при удалении Refresh Token из базы данных refresh token id: {token.Id}");
                   
                
            
        }
    }
}
