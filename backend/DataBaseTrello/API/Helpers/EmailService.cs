using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
namespace API.Helpers
{
    public class EmailService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public EmailService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

    }
}
