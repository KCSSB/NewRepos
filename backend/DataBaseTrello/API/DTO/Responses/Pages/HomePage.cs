namespace API.DTO.Responses.Pages
{
    public record HomePage
    {
        public List<SummaryProjectResponse> SummaryProject { get; set; }
        public string UserAvatarUrl { get; set; }
    }
}
