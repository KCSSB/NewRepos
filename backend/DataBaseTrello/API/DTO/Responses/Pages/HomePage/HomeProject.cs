namespace API.DTO.Responses.Pages.HomePage
{
    public record HomeProject
    {
        public int ProjectId   { get; set; }
        public string ProjectName { get; set; }
        public string ProjectImageUrl { get; set; }
        public int CountProjectUsers { get; set; }
        public HomeProjectLeader ProjectLeader { get; set; }
    }
}
