using DataBaseInfo.models;

namespace API.DTO.Responses.Pages
{
    public record class SettingsPage
    {
        public string UserEmail { get; set; }
        public string UserAvatarUrl { get; set; }
        public string FirstUserName { get; set; }
        public string LastUserName { get; set; }
        public Sex Sex { get; set; }
        public Guid InviteId { get; set; }

    }
}
