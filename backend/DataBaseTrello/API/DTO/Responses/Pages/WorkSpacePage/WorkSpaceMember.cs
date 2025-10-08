namespace API.DTO.Responses.Pages.WorkSpacePage
{
    public record WorkSpaceMember
    {
        public int UserId { get; set; }
        public int ProjectUserId    { get; set; }
        public string ProjectRole {  get; set; }
        public int? MemberId { get; set; }
        public string? BoardRole { get; set; }
        public string UserAvatar { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
    }
}
