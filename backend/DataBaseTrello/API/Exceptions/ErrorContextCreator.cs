using System.Net;
using System.Runtime.CompilerServices;
using API.Constants;
using API.Exceptions.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;


namespace API.Exceptions
{
    public class ErrorContextCreator
    {
        public readonly string ServiceName;
        public ErrorContextCreator(string _serviceName)
        {
            ServiceName = _serviceName;
        }
        public ErrorContext BadRequest(string loggerMessage, [CallerMemberName] string operationName = null)
        {
            return new ErrorContext(ServiceName,
                operationName,
                HttpStatusCode.BadRequest,
                UserExceptionMessages.BadRequestExceptionMessage,
                loggerMessage);
        }
        public ErrorContext Forbidden(string loggerMessage, [CallerMemberName] string operationName = null)
        {
            return new ErrorContext(ServiceName,
                operationName,
                HttpStatusCode.Forbidden,
                UserExceptionMessages.ForbiddenExceptionMessage,
                loggerMessage);
        }
        public ErrorContext NotFound(string loggerMessage, [CallerMemberName] string operationName = null)
        {
            return new ErrorContext(ServiceName,
                operationName,
                HttpStatusCode.NotFound,
                UserExceptionMessages.InternalExceptionMessage,
                loggerMessage);
        }
        public ErrorContext Conflict(string loggerMessage, [CallerMemberName] string operationName = null)
        {
            return new ErrorContext(ServiceName,
                operationName,
                HttpStatusCode.Conflict,
                UserExceptionMessages.ConflictExceptionMessage,
                loggerMessage);
        }
        public ErrorContext InternalServerError(string loggerMessage, [CallerMemberName] string operationName = null)
        {
            return new ErrorContext(ServiceName,
                operationName,
                HttpStatusCode.InternalServerError,
                UserExceptionMessages.InternalExceptionMessage,
                loggerMessage);
        }
        public ErrorContext Unauthorized(string loggerMessage, [CallerMemberName] string operationName = null)
        {
            return new ErrorContext(ServiceName,
                operationName,
                HttpStatusCode.Unauthorized,
                UserExceptionMessages.UnauthorizedExceptionMessage,
                loggerMessage);
        }

    }
}
