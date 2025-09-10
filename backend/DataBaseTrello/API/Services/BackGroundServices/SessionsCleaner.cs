using API.Exceptions.ContextCreator;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;

namespace API.Services.BackGroundServices
{
    public class SessionsCleaner : BackgroundService
    {
        private readonly ILogger<SessionsCleaner> _logger;
        private readonly  IServiceScopeFactory _scopeFactory;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;



        public SessionsCleaner(ILogger<SessionsCleaner> logger, IServiceScopeFactory scopeFactory, IErrorContextCreatorFactory errCreatorFactory)
        {
            _errCreatorFactory = errCreatorFactory;
            _logger = logger;
            _scopeFactory = scopeFactory;
           
        }
private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(SessionsCleaner));
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                
                
                    try
                    {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var expiredSessions = await context.Sessions
                            .Where(t => t.ExpiresAt < DateTime.UtcNow || t.IsRevoked)
                            .ToListAsync(ct);

                        if (expiredSessions.Any())
                        {
                            context.Sessions.RemoveRange(expiredSessions);
                            await context.SaveChangesAsync(ct);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при очистке токенов");
                    }

                
                await Task.Delay(TimeSpan.FromHours(24), ct);
            }

        }
    }
}
