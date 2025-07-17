using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task<int> SaveChangesWithContextAsync(this DbContext context, string serviceName, string operationName, string message)
        {

       try
    {
               
                return await context.SaveChangesAsync();
                
    }
    catch (DbUpdateException ex)
    {
        ex.Data.Add("service", serviceName);
        ex.Data.Add("operation", operationName);
                ex.Data.Add("message", message);
        throw;
    }
        }
    }
}
