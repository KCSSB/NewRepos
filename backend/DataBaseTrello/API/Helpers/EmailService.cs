using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using Microsoft.Extensions.Options;
using API.Constants;
using API.Exceptions;
namespace API.Helpers
{
    public class EmailService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IOptions<EmailService> _settings;
        private readonly ErrorContextCreator _errCreator;
        public EmailService(IDbContextFactory<AppDbContext> contextFactory, IOptions<EmailService> settings)
        {
            _contextFactory = contextFactory;
            _settings = settings;
            _errCreator = new ErrorContextCreator(ServiceName.EmailService);
        }
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
