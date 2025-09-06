using MimeKit;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using Microsoft.Extensions.Options;
using API.Exceptions.ContextCreator;
using API.Services.Helpers.Interfaces;
namespace API.Services.Helpers.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailService> _settings;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private readonly AppDbContext _context;
        private ErrorContextCreator? _errorContextCreator;


        public EmailService(AppDbContext context, IOptions<EmailService> settings, IErrorContextCreatorFactory errCreatorFactory)
        {
            _errCreatorFactory = errCreatorFactory;
            _settings = settings;
            _context = context;
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
