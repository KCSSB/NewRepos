using API.Exceptions.ContextCreator;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;

namespace API.Services.BackGroundServices
{
    public class SessionsCleaner : BackgroundService
    {
        private readonly ILogger<SessionsCleaner> _logger;
        private readonly  IServiceProvider _serviceProvider;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;



        public SessionsCleaner(ILogger<SessionsCleaner> logger, IServiceProvider serviceProvider, IErrorContextCreatorFactory errCreatorFactory)
        {
            _errCreatorFactory = errCreatorFactory;
            _logger = logger;
            _serviceProvider = serviceProvider;
           
        }
private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(SessionsCleaner));
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                
                
                    try
                    {
                    using var scope = _serviceProvider.CreateScope();
                    var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var expiredSessions = await _context.Sessions
                            .Where(t => t.ExpiresAt < DateTime.UtcNow || t.IsRevoked)
                            .ToListAsync(ct);

                        if (expiredSessions.Any())
                        {
                            _context.Sessions.RemoveRange(expiredSessions);
                            await _context.SaveChangesAsync(ct);
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
