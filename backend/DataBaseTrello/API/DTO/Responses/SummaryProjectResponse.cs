namespace API.DTO.Responses
{
    public record SummaryProjectResponse
    {
        public Guid ProjectId   { get; set; }
        public string ProjectName { get; set; }
        public string ProjectImageUrl { get; set; }
        public int CountProjectUsers { get; set; }
        public ProjectLeaderResponse ProjectLeader { get; set; }
    }
}
