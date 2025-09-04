using System.Security.Claims;
using API.DTO.Domain;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Extensions;
using API.Services.Helpers;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace API.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;
        public SessionValidationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory, IDbContextFactory<AppDbContext> contextFactory, IErrorContextCreatorFactory errCreatorFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
            _contextFactory = contextFactory;
            _errCreatorFactory = errCreatorFactory;
        }
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(SessionValidationMiddleware));
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
            
                

            var refreshToken = context.Request.Cookies["refreshToken"];
                if (refreshToken == null)
                    throw new AppException(_errCreator.Unauthorized("Ошибка при получении refreshToken из Cookie"));
 
            int userId = context.User.GetUserId();
            string deviceId = context.User.GetDeviceId();

            bool? sessionIsRevoked = await SessionIsRevokedAsync(userId, deviceId, refreshToken);
                if (sessionIsRevoked == null)
                    throw new AppException(_errCreator.Unauthorized("Сессия не существует"));
                var sessionExist = !sessionIsRevoked.Value;
                if (sessionExist)
                    await _next(context);
                else
                    throw new AppException(_errCreator.Unauthorized($"Попытка входа в сессию токен которой был отозван userId: {userId} deviceId:{deviceId}"));
            }
            else
            {
                await _next(context);
            }
        }
        private async Task<bool?> SessionIsRevokedAsync(int userId, string deviceId, string refreshToken)
        {
            IServiceScope scope = _scopeFactory.CreateScope();
        
            var redis = scope.ServiceProvider.GetService<RedisService>();
            var hashService = scope.ServiceProvider.GetService<HashService>();

            SessionData? session = await redis.Session.SafeGetSessionAsync(userId, deviceId);
            if (session != null)
            {
                if (hashService.VerifyToken(refreshToken,session.HashedToken))
                    return session.IsRevoked;
                else
                    throw new AppException(_errCreator.Unauthorized($"Неверный refresh Token"));
            }

            using var context = await _contextFactory.CreateDbContextAsync();

            var dbSessions = await context.Sessions
                .Where(s => s.UserId == userId && s.DeviceId == Guid.Parse(deviceId))
                .ToListAsync();

            var dbSession = dbSessions.FirstOrDefault(s => hashService.VerifyToken(refreshToken, s.Token));

            if (dbSession != null)
                return dbSession.IsRevoked;
            return null;
        }
    }
    public static class SessionValidationMiddlewareExtension
    {
        public static IApplicationBuilder UseSessionValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionValidationMiddleware>();
        }
    }
}
