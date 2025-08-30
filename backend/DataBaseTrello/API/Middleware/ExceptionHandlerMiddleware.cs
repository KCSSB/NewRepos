using System.Net;
using System.Text;
using API.DTO.Responses;
using API.Exceptions.Context;
using API.Middleware;


namespace API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate nect, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = nect;
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
                var exceptionMessage = new StringBuilder();
                if (ex.Context.Service != null)
                    exceptionMessage.AppendLine($"Service: {ex.Context.Service}");
                if (ex.Context.Operation != null)
                    exceptionMessage.AppendLine($"Operation: {ex.Context.Operation}");
                if (ex.Context.StatusCode != null)
                    exceptionMessage.AppendLine($"StatusCode: {ex.Context.StatusCode}");
                if (ex.Context.LoggerMessage != null)
                    exceptionMessage.AppendLine($"LoggerMessage: {ex.Context.LoggerMessage}");

                    await HandleExceptionAsync(context,
                        exceptionMessage.ToString(),
                        (HttpStatusCode)ex.Context.StatusCode,
                        ex.Context.UserMessage);
                
                
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(context,
                          "Произошла непредвиденная ошибка",
                          HttpStatusCode.BadRequest,
                          "Произошла неизвестная ошибка, повторите попытку позже");
            }
        }
        private async Task HandleExceptionAsync(HttpContext context,string exMessage, HttpStatusCode httpStatusCode, string message)
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
                Message = message,
                StatusCode = (int)httpStatusCode
            };

            await response.WriteAsJsonAsync(errorResponse);

            
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
