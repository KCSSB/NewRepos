using DataBaseInfo.models;

namespace API.DTO.Responses
{
    public record class UpdateUserResponse
    {
        public string? FirstUserName { get; set; }
        public string? LastUserName { get; set; }
        public Sex? Sex { get; set; }
    }
}
