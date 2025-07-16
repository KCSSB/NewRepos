using API.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
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
            catch(Exception e)
            {
                //Логирование
                
            }
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
