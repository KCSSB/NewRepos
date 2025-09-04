using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Constants;
using API.Exceptions.ContextCreator;
using API.Middleware;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Services.BackGroundServices
{
    public class RefreshTokensCleaner : BackgroundService
    {
        private readonly ILogger<RefreshTokensCleaner> _logger;
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;



        public RefreshTokensCleaner(ILogger<RefreshTokensCleaner> logger, IDbContextFactory<AppDbContext> dbContextFactory, IErrorContextCreatorFactory errCreatorFactory)
        {
        _errCreatorFactory = errCreatorFactory;
            _logger = logger;
            _dbContextFactory = dbContextFactory;
           
        }
private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(RefreshTokensCleaner));
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await using (var dbContext = await _dbContextFactory.CreateDbContextAsync(ct))
                {
                    try
                    {
                        var expiredTokens = await dbContext.Sessions
                            .Where(t => t.ExpiresAt < DateTime.UtcNow || t.IsRevoked)
                            .ToListAsync(ct);

                        if (expiredTokens.Any())
                        {
                            dbContext.Sessions.RemoveRange(expiredTokens);
                            await dbContext.SaveChangesAsync(ct);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при очистке токенов");
                    }

                }
                await Task.Delay(TimeSpan.FromHours(24), ct);
            }

        }
    }
}
