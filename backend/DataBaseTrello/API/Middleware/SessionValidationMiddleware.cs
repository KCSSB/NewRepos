using System.Security.Claims;
using API.DTO.Domain;
using API.Exceptions;
using API.Exceptions.Context;
using API.Extensions;
using API.Helpers;
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

        public SessionValidationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory, IDbContextFactory<AppDbContext> contextFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
            _contextFactory = contextFactory;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
            var scope = _scopeFactory.CreateScope();
            var errCreator = scope.ServiceProvider.GetService<ErrorContextCreator>();
            var refreshToken = context.Request.Cookies["refreshToken"];
                if (refreshToken == null)
                    throw new AppException(errCreator.Unauthorized("Ошибка при получении refreshToken из Cookie"));
 
            int userId = context.User.GetUserId();
            string deviceId = context.User.GetDeviceId();

            bool? sessionIsRevoked = await SessionIsRevokedAsync(userId, deviceId, refreshToken, scope);
                if (sessionIsRevoked == null)
                    throw new AppException(errCreator.Unauthorized("Сессия не существует"));
                var sessionExist = !sessionIsRevoked.Value;
                if (sessionExist)
                    await _next(context);
                else
                    throw new AppException(errCreator.Unauthorized($"Попытка входа в сессию токен которой был отозван userId: {userId} deviceId:{deviceId}"));
            }
            else
            {
                await _next(context);
            }
        }
        private async Task<bool?> SessionIsRevokedAsync(int userId, string deviceId, string refreshToken,IServiceScope scope)
        {
            
            var errCreator = scope.ServiceProvider.GetService<ErrorContextCreator>();
            var redis = scope.ServiceProvider.GetService<RedisService>();
            var hashService = scope.ServiceProvider.GetService<HashService>();

            SessionData? session = await redis.Session.GetSessionAsync(userId, deviceId);
            if (session != null)
            {
                if (hashService.VerifyToken(refreshToken,session.HashedToken))
                    return session.IsRevoked;
                else
                    throw new AppException(errCreator.Unauthorized($"Неверный refresh Token"));
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
