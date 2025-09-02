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
using API.Constants;
using API.Exceptions;
using StackExchange.Redis;
namespace API.Helpers
{
    public class JWTServices
    {
        private readonly IOptions<AuthSettings> _options;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly HashService _hashService;
        private readonly ILogger<JWTServices> _logger;
        private readonly ErrorContextCreator _errCreator;
        private readonly RedisService _redis;
        public JWTServices(IOptions<AuthSettings> options,
            IDbContextFactory<AppDbContext> contextFactory,
            HashService hashService,
            ILogger<JWTServices> logger,
            RedisService redis)
        {
            _contextFactory = contextFactory;
            _hashService = hashService;
            _options = options;
            _logger = logger;
            _errCreator = new ErrorContextCreator(ServiceName.JWTServices);
            _redis = redis;
        }
        public string GenerateAccessToken(User user, string? deviceId)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim("FirstName", user.FirstName),
                new Claim("SecondName", user.SecondName),
               new Claim("Sex", SexHelper.GetSexDisplay(user.Sex)),
               new Claim("InviteId", user.InviteId.ToString()),
                new Claim("UserEmail", user.UserEmail),
                new Claim("Avatar", user.Avatar),
                new Claim("DeviceId", deviceId.ToString()),
            };
            
            var jwtToken = new JwtSecurityToken(
                expires: DateTime.UtcNow.Add(_options.Value.AccessTokenExpires),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey)), SecurityAlgorithms.HmacSha256));
         
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        
        public async Task<string> CreateRefreshTokenAsync(User user,string? deviceId, AppDbContext? context = null)
        {
            var ownContext = context == null;
            if (ownContext)
                context = await _contextFactory.CreateDbContextAsync();

            var token = Guid.NewGuid().ToString();
            var hashedToken = _hashService.HashToken(token);
            var session = new Session
                  {
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.Add(_options.Value.RefreshTokenExpires),
                    Token = hashedToken,
                    IsRevoked = false,
                    UserId = user.Id,
                    DeviceId = Guid.Parse(deviceId),
                  };
                    user.Sessions.Add(session);
              
            await context.Sessions.AddAsync(session);

            await context.SaveChangesWithContextAsync("Ошибка при сохранении Hashed Refresh Token");
            if (ownContext)
                await context.DisposeAsync();
            _redis.Session.SetSessionAsync(hashedToken,user.Id, deviceId);
            return token;
            }
          
        
        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken, int userId, string deviceId)
        {
            _redis.Session.RevokeSessionAsync(userId, deviceId);
            using var context = _contextFactory.CreateDbContext();
            
                var hashToken = _hashService.HashToken(refreshToken);

            var storedToken = await context.Sessions
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => hashToken == rt.Token);

            RefTokenIsValid(storedToken);
            storedToken.IsRevoked = true;

            var createToken = CreateRefreshTokenAsync(storedToken.User, deviceId);
            var newAccessToken = GenerateAccessToken(storedToken.User, deviceId);
            var token = await createToken;
            return (newAccessToken, token);
            }

            public void RefTokenIsValid(Session? token)
            {
            if (token == null)
                throw new AppException(_errCreator.Unauthorized("RefreshToken не существует"));
            else if (token.IsRevoked)
                throw new AppException(_errCreator.Unauthorized("RefreshToken был отозван"));
            else if (token.ExpiresAt < DateTime.UtcNow)
                throw new AppException(_errCreator.Unauthorized("RefreshToken истёк"));
            }
            
    
        
        public async Task RevokeRefreshTokenAsync(string refreshToken, int userId,string deviceId) // Где то тут может быть ошибка, поищи
        {
            await _redis.Session.RevokeSessionAsync(userId, deviceId);
            var contextTask = _contextFactory.CreateDbContextAsync();
           
            var hashedRequestToken = _hashService.HashToken(refreshToken);
            using var context = await contextTask;
            var token = await context.Sessions.FirstOrDefaultAsync(s => 
            s.Token == hashedRequestToken
            && s.UserId == userId
            && s.DeviceId == Guid.Parse(deviceId));
            
            if (token == null)
                throw new AppException(_errCreator.Unauthorized("В базе не найден refresh token, соответствующий значению из cookie при попытке выйти из аккаунта"));
            token.IsRevoked = true;
            await context.SaveChangesWithContextAsync($"Ошибка при удалении Refresh Token из базы данных refresh token id: {token.Id}");    
            
        }
    }
}
