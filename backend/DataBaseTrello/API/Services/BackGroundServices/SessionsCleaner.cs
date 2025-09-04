using API.Exceptions.ContextCreator;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;

namespace API.Services.BackGroundServices
{
    public class SessionsCleaner : BackgroundService
    {
        private readonly ILogger<SessionsCleaner> _logger;
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;



        public SessionsCleaner(ILogger<SessionsCleaner> logger, IDbContextFactory<AppDbContext> dbContextFactory, IErrorContextCreatorFactory errCreatorFactory)
        {
            _errCreatorFactory = errCreatorFactory;
            _logger = logger;
            _dbContextFactory = dbContextFactory;
           
        }
private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(SessionsCleaner));
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await using (var dbContext = await _dbContextFactory.CreateDbContextAsync(ct))
                {
                    try
                    {
                        var expiredSessions = await dbContext.Sessions
                            .Where(t => t.ExpiresAt < DateTime.UtcNow || t.IsRevoked)
                            .ToListAsync(ct);

                        if (expiredSessions.Any())
                        {
                            dbContext.Sessions.RemoveRange(expiredSessions);
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
