namespace API.DTO.Responses
{
    public record ProjectLeaderResponse
    {
        public Guid ProjectLeaderId { get; set; }
        public string ProjectLeaderName { get; set;}
        public string ProjectLeaderImageUrl { get; set; }
    }
}
