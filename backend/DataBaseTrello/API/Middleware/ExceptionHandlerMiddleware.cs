using System.Net;
using System.Text;
using System.Text.Json;
using API.DTO.Responses;
using API.Extensions;
using API.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

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
            catch(DbUpdateException ex)
            {
               var exceptionMessage = new StringBuilder();
                if (ex.TryGetFromExceptionData("service", out string service))
                    exceptionMessage.AppendLine($"Service: {service}");
                if (ex.TryGetFromExceptionData("operation", out string operation))
                    exceptionMessage.AppendLine($"Operation: {operation}");
                if (ex.TryGetFromExceptionData("message", out string message))
                    exceptionMessage.AppendLine($"Message: {message}");
                if (exceptionMessage.Length == 0)
                    exceptionMessage.Append("Контекст ошибки повреждён");
                await HandleExceptionAsync(context, exceptionMessage.ToString(), HttpStatusCode.NotFound, "Не найдено");
                
            }
        }
        private async Task HandleExceptionAsync(HttpContext context,string exMessage, HttpStatusCode httpStatusCode, string message)
        {
            _logger.LogError(exMessage);
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
