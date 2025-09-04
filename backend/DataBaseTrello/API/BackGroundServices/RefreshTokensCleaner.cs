using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Constants;
using API.Exceptions;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.BackGroundServices
{
    public class RefreshTokensCleaner : BackgroundService
    {
        private readonly ILogger<RefreshTokensCleaner> _logger;
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly ErrorContextCreator _errCreator;

        public RefreshTokensCleaner(ILogger<RefreshTokensCleaner> logger, IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _errCreator = new ErrorContextCreator(ServiceName.RefreshTokensCleaner);
        }
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
