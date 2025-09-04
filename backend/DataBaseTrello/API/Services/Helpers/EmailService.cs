using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using Microsoft.Extensions.Options;
using API.Constants;
using API.Middleware;
using API.Exceptions.ContextCreator;
namespace API.Services.Helpers
{
    public class EmailService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IOptions<EmailService> _settings;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;


        public EmailService(IDbContextFactory<AppDbContext> contextFactory, IOptions<EmailService> settings, IErrorContextCreatorFactory errCreatorFactory)
        {
        _errCreatorFactory = errCreatorFactory;
            _contextFactory = contextFactory;
            _settings = settings;
        }
private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(EmailService));
        public async Task<MimeMessage> CreateMessageAsync()
        {
            return new MimeMessage
            {

            };
        }
        public async Task SendMessageAsync()
        {
            return;
        }
        public async Task SendInviteMessage(int projectId, int userId)
        {
            var msg = await CreateMessageAsync();
            //await SendMessageAsync(msg);
        }
        

    }
}
