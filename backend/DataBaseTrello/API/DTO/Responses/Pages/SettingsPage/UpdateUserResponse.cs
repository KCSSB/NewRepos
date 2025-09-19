using DataBaseInfo.models;

namespace API.DTO.Responses.Pages.SettingsPage
{
    public record class UpdateUserResponse
    {
        public string? FirstUserName { get; set; }
        public string? LastUserName { get; set; }
        public Sex? Sex { get; set; }
    }
}
