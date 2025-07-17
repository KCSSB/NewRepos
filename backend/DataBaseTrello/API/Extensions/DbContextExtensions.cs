using System.Net;
using API.Exceptions.ErrorContext;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task<int> SaveChangesWithContextAsync(this DbContext context,
            string serviceName,
            string operationName,
            string loggerMessage,
            string userMessage,
            HttpStatusCode statusCode)
        {

       try
    {
     
                return await context.SaveChangesAsync();
                
    }
    catch (DbUpdateException ex)
    {

                throw new AppException(new ErrorContext(serviceName,
                    operationName,
                    statusCode,
                    userMessage,
                    loggerMessage), ex);
    }
        }
    }
}
