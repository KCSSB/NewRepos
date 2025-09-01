using System.Security.Claims;
using API.DTO.Domain;
using API.Exceptions;
using API.Exceptions.Context;
using API.Extensions;
using API.Helpers;

namespace API.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;
        public SessionValidationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {

                var scope = _scopeFactory.CreateScope();
            var errCreator = scope.ServiceProvider.GetService<ErrorContextCreator>();
            
            int userId = context.User.GetUserId();
            string deviceId = context.User.GetDeviceId();

            bool sessionIsRevoked = await SessionIsRevokedAsync(userId, deviceId);
            if (!sessionIsRevoked)
                await _next(context);
            else
                throw new AppException(errCreator.Unauthorized($"Попытка входа в сессию токен которой был отозван userId: {userId} deviceId:{deviceId}"));
            }
            else
            {
                await _next(context);
            }
        }
        private async Task<bool> SessionIsRevokedAsync(int userId, string deviceId)
        {
            var scope = _scopeFactory.CreateScope();
            var redis = scope.ServiceProvider.GetService<RedisService>();

            SessionData session = await redis.Session.GetSessionAsync(userId, deviceId);
            return session.IsRevoked;
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
