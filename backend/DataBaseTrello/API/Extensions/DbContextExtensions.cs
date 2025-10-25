using System.Net;
using System.Runtime.CompilerServices;
using API.Constants;
using API.Exceptions;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace API.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task<int> SaveChangesWithContextAsync(this DbContext context,
            string logMessage,string serviceName, string? operationName)
        {
            var _errCreator = new ErrorContextCreator(serviceName);
            try
             {
                return await context.SaveChangesAsync();
             }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is PostgresException pgEx)
                {
                    switch (pgEx.SqlState)
                    {
                        case "23505":
                            throw new AppException(_errCreator.Conflict("Нарушение уникальности ключа")); // уникальный ключ
                        case "23503":
                            throw new AppException(_errCreator.BadRequest("Нарушение внешнего ключа"));
                        case "23502":
                            throw new AppException(_errCreator.BadRequest("попытка поместить NULL в поле NOT NULL"));
                        case "23514":
                            throw new AppException(_errCreator.BadRequest("Нарушение CHECK Ограничения"));
                        default:
                            throw new AppException(_errCreator.InternalServerError("Произошла внутренняя ошибка сервера во время сохранения изменений"));
                    }
                }
                throw new AppException(_errCreator.InternalServerError("Произошла внутренняя ошибка сервера во время сохранения изменений"));
            }
        }
    }
}
