using System.Net;
using System.Text;
using API.DTO.Responses;
using API.Exceptions.Context;
using API.Middleware;
using StackExchange.Redis;


namespace API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
            await _next(context);
            }
            catch(AppException ex)
            {
                var exceptionMessage = ConstructExceptionMessage(ex);

                    await HandleExceptionAsync(context,
                        exceptionMessage,
                        (HttpStatusCode)ex.Context.StatusCode);
            }
            catch(RedisException ex)
            {
                _logger.LogWarning(ex.Message);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(context,
                          "Произошла непредвиденная ошибка",
                          HttpStatusCode.InternalServerError);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context,string exMessage, HttpStatusCode httpStatusCode)
        {
            if((int)httpStatusCode/100 == 2 || (int)httpStatusCode / 100 == 3)
                _logger.LogWarning(exMessage);
            else
            {
                _logger.LogError('\n'+exMessage);
            }


            HttpResponse response = context.Response;

            response.ContentType = "application/json";
            response.StatusCode = (int)httpStatusCode;
            ErrorResponse errorResponse = new()
            {
                StatusCode = (int)httpStatusCode
            };

            await response.WriteAsJsonAsync(errorResponse);
        }
        private string ConstructExceptionMessage(AppException ex)
        {
            var exceptionMessage = new StringBuilder();
            if (ex.Context.Service != null)
                exceptionMessage.AppendLine($"Service: {ex.Context.Service}");
            if (ex.Context.Operation != null)
                exceptionMessage.AppendLine($"Operation: {ex.Context.Operation}");
            if (ex.Context.StatusCode != null)
                exceptionMessage.AppendLine($"StatusCode: {ex.Context.StatusCode}");
            if (ex.Context.LoggerMessage != null)
                exceptionMessage.AppendLine($"LoggerMessage: {ex.Context.LoggerMessage}");
            return exceptionMessage.ToString();
        }
    }
}
public static class ExceptionHandlingMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder) 
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}

