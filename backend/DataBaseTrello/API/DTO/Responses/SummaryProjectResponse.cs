namespace API.DTO.Responses
{
    public record SummaryProjectResponse
    {
        public Guid ProjectId   { get; set; }
        public string ProjectName { get; set; }
        public string ProjectImageUrl { get; set; }
        public string CountProjectUsers { get; set; }
        public Guid ProjectLeaderId { get; set; }
        public string ProjectLeaderName { get; set; }
        public string ProjectLeaderImageUrl { get; set; }
    }
}
