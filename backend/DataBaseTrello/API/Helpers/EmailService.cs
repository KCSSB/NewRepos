using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using Microsoft.Extensions.Options;
namespace API.Helpers
{
    public class EmailService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IOptions<EmailService> _settings;
        public EmailService(IDbContextFactory<AppDbContext> contextFactory, IOptions<EmailService> settings)
        {
            _contextFactory = contextFactory;
            _settings = settings;
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
        public async Task SendInviteMessage(Guid projectId, Guid userId)
        {
            var msg = await CreateMessageAsync();
            //await SendMessageAsync(msg);
        }
        

    }
}
