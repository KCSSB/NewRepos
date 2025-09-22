namespace API.DTO.Responses.Pages.HallPage
{
    public record HallProjectUser
    {
        public int ProjectUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProjectRole { get; set; }

    }
}
