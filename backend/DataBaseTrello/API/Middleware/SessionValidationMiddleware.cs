using System.Security.Claims;
using API.DTO.Domain;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Extensions;
using API.Services.Application.Implementations;
using API.Services.Helpers;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using OpenTelemetry;
using Org.BouncyCastle.Asn1.Ocsp;

namespace API.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private ErrorContextCreator? _errorContextCreator;
        public SessionValidationMiddleware(RequestDelegate next,
            IServiceScopeFactory scopeFactory, 
            IErrorContextCreatorFactory errCreatorFactory)
        {
            _next = next;
            _errCreatorFactory = errCreatorFactory;
            _scopeFactory = scopeFactory;
        }
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(SessionValidationMiddleware));
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var scope = _scopeFactory.CreateAsyncScope();
                var _sessionService = scope.ServiceProvider.GetService<SessionService>();
            var refreshToken = context.Request.Cookies["refreshToken"];
                if (refreshToken == null)
                    throw new AppException(_errCreator.Unauthorized("Ошибка при получении refreshToken из Cookie"));
 
            int userId = context.User.GetUserId();
            string deviceId = context.User.GetDeviceId();

            bool? sessionIsRevoked = await _sessionService.SessionIsRevokedAsync(userId, deviceId, refreshToken);
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
        
    }
    public static class SessionValidationMiddlewareExtension
    {
        public static IApplicationBuilder UseSessionValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionValidationMiddleware>();
        }
    }
}
