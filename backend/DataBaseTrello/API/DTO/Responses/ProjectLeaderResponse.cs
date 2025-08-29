namespace API.DTO.Responses
{
    public record ProjectLeaderResponse
    {
        public int ProjectLeaderId { get; set; }
        public string ProjectLeaderName { get; set;}
        public string ProjectLeaderImageUrl { get; set; }
    }
}
