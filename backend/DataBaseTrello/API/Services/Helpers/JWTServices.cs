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
using StackExchange.Redis;
using System.Collections.Generic;
using API.Middleware;
using API.Exceptions.ContextCreator;
namespace API.Services.Helpers
{
    public class JWTServices
    {
        private readonly IOptions<AuthSettings> _options;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly HashService _hashService;
        private readonly RedisService _redis;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;


        public JWTServices(IOptions<AuthSettings> options,
            IDbContextFactory<AppDbContext> contextFactory,
            HashService hashService,
            ILogger<JWTServices> logger,
            RedisService redis, IErrorContextCreatorFactory errCreatorFactory)
        {
        _errCreatorFactory = errCreatorFactory;
            _contextFactory = contextFactory;
            _hashService = hashService;
            _options = options;
            _redis = redis;
        }
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(JWTServices));
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

            _ = _redis.Session.SafeSetSessionAsync(hashedToken, user.Id, deviceId);

                return token;
            }
          
        
        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken, int userId, string deviceId)
        {
            _ = _redis.Session.SafeRevokeSessionAsync(userId, deviceId);
            using var context = _contextFactory.CreateDbContext();
            
                var hashToken = _hashService.HashToken(refreshToken);

            var oldSession = await context.Sessions
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => hashToken == rt.Token);

            RefTokenIsValid(oldSession);
            oldSession.IsRevoked = true;

            var createToken = CreateRefreshTokenAsync(oldSession.User, deviceId);
            var newAccessToken = GenerateAccessToken(oldSession.User, deviceId);
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
        public async Task RevokeSessionAsync(int userId,string deviceId) 
        {
            _ = _redis.Session.SafeRevokeSessionAsync(userId, deviceId);
            using var context =await _contextFactory.CreateDbContextAsync();
           
            var session = await context.Sessions.FirstOrDefaultAsync(s => 
            s.UserId == userId
            && s.DeviceId == Guid.Parse(deviceId));
            
            if (session == null)
                throw new AppException(_errCreator.Unauthorized("В базе не найдена сессия соответствующий значению из cookie при попытке выйти из аккаунта"));
            session.IsRevoked = true;
            await context.SaveChangesWithContextAsync($"Ошибка при удалении сессии из базы данных session id: {session.Id}");    
            
        }
        private async Task RevokeSessionRangeAsync(List<Session> sessions)
        {
            if (sessions.Count == 0)
                return;
            using var context = await _contextFactory.CreateDbContextAsync();
            var redisTasks = sessions.Select(s => _redis.Session.SafeRevokeSessionAsync(s.UserId, s.DeviceId.ToString())).ToList();
            foreach (var session in sessions)   
                session.IsRevoked = true;
            try
            {await Task.WhenAll(redisTasks);}
            catch(Exception ex)
            {
            }
            await context.SaveChangesWithContextAsync($"Ошибка при удалении сессий из бд");
        }
        private async Task RevokeSessionToTheLimitAsync(List<Session> sessions,List<Session> forRevoke, Guid deviceId,bool hasDevice = false, int limit = 3)
        {
            if (sessions.Count < limit)
                return;
           if(hasDevice==true)
                forRevoke.AddRange(sessions.Skip(limit+1));
           else
                forRevoke.AddRange(sessions.Skip(limit));
            await RevokeSessionRangeAsync(forRevoke);
        
        }
        private bool RevokeThisDeviceSessions(List<Session> sessions,List<Session> forRevoke, Guid deviceId, int limit = 3)
        {
            bool hasDeviceId = false;
            int i = 0;
            foreach(var session in sessions)
                {
                if (i > 3)
                    return hasDeviceId;
                if (session.DeviceId == deviceId)
                {
                    forRevoke.Add(session);
                    hasDeviceId = true;
                }
                i++;
            }
            
            return hasDeviceId;
        }
        public async Task RevokeSessionsAsync(List<Session> sessions, Guid deviceId, int limit = 3)
        {
            List<Session> forRevoked = new List<Session>();
            bool hasDeviceId = RevokeThisDeviceSessions(sessions, forRevoked, deviceId, limit);

            if(sessions.Count>limit)
                await RevokeSessionToTheLimitAsync(sessions, forRevoked, deviceId, hasDeviceId, limit);
        }
    }
}
