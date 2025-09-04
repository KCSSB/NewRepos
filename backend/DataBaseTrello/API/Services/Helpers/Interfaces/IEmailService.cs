using MimeKit;

namespace API.Services.Helpers.Interfaces
{
    public interface IEmailService
    {
        Task<MimeMessage> CreateMessageAsync();
        Task SendInviteMessage(int projectId, int userId);
        Task SendMessageAsync();
    }
}