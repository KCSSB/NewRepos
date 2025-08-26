using DataBaseInfo.models;

namespace API.DTO.Domain
{
    public class UpdateUserModel
    {
        public string? FirstUserName { get; set; }

        public string? LastUserName { get; set; }

        public Sex? Sex { get; set; }
    }
}
