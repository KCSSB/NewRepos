using API.DTO.Domain;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Services.Application.Interfaces;
using API.Services.Helpers.Implementations;
using API.Services.Helpers.Interfaces;
using API.Services.Helpers.Interfaces.Redis;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Application.Implementations
{
    public class SessionService : ISessionService
    {
        private readonly IRedisService _redis;
        private readonly IHashService _hashService;
        private readonly IErrorContextCreatorFactory errorContextCreatorFactory;
        private ErrorContextCreator errCreator;
        private readonly AppDbContext _context;
        public SessionService(IRedisService redis, 
            IHashService hashService, 
            IErrorContextCreatorFactory _errorCreatorFactory,
            AppDbContext context)
            {
            _redis = redis;
            _hashService = hashService;
            }
        private ErrorContextCreator _errCreator => errCreator ??= errorContextCreatorFactory.Create(nameof(ISessionService));
        public async Task<bool?> SessionIsRevokedAsync(int userId, string deviceId, string refreshToken)
        {
            SessionData? session = await _redis.Session.SafeGetSessionAsync(userId, deviceId);
            if (session != null)
            {
                if (_hashService.VerifyToken(refreshToken, session.HashedToken))
                    return session.IsRevoked;
                else
                    throw new AppException(_errCreator.Unauthorized($"Неверный refresh Token"));
            }
            var dbSessions = await _context.Sessions
                .Where(s => s.UserId == userId && s.DeviceId == Guid.Parse(deviceId))
                .ToListAsync();

            var dbSession = dbSessions.FirstOrDefault(s => _hashService.VerifyToken(refreshToken, s.Token));

            if (dbSession != null)
                return dbSession.IsRevoked;
            return null;
        }
    }
}
