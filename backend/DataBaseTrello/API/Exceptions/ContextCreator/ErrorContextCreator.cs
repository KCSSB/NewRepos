using System.Net;
using System.Runtime.CompilerServices;
using API.Constants;
using API.Exceptions.Context;

namespace API.Exceptions.ContextCreator
{
    public class ErrorContextCreator
    {
        public readonly string ServiceName;
        public readonly string OperationName = null;
        public ErrorContextCreator(string _serviceName)
        {
            ServiceName = _serviceName;
        }
        public ErrorContextCreator(string _serviceName, string operationName)
        {
            ServiceName = _serviceName;
            OperationName = operationName;
        }
        public ErrorContext BadRequest(string loggerMessage, [CallerMemberName] string operationName = null)
        {
            if(OperationName != null) operationName = OperationName;
            return new ErrorContext(ServiceName,
                operationName,
                HttpStatusCode.BadRequest,
                loggerMessage);
        }
        public ErrorContext Forbidden(string loggerMessage, [CallerMemberName] string operationName = null)
        {
            if (OperationName != null) operationName = OperationName;
            return new ErrorContext(ServiceName,
                operationName,
                HttpStatusCode.Forbidden,
                loggerMessage);
        }
        public ErrorContext NotFound(string loggerMessage, [CallerMemberName] string operationName = null)
        {
            if (OperationName != null) operationName = OperationName;
            return new ErrorContext(ServiceName,
                operationName,
                HttpStatusCode.NotFound,
                loggerMessage);
        }
        public ErrorContext Conflict(string loggerMessage, [CallerMemberName] string operationName = null)
        {
            if (OperationName != null) operationName = OperationName;
            return new ErrorContext(ServiceName,
                operationName,
                HttpStatusCode.Conflict,
                loggerMessage);
        }
        public ErrorContext InternalServerError(string loggerMessage, [CallerMemberName] string operationName = null)
        {
            if (OperationName != null) operationName = OperationName;
            return new ErrorContext(ServiceName,
                operationName,
                HttpStatusCode.InternalServerError,
                loggerMessage);
        }
        public ErrorContext Unauthorized(string loggerMessage, [CallerMemberName] string operationName = null)
        {
            if (OperationName != null) operationName = OperationName;
            return new ErrorContext(ServiceName,
                operationName,
                HttpStatusCode.Unauthorized,
                loggerMessage);
        }

    }
}
