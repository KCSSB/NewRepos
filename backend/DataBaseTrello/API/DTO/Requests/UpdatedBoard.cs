namespace API.DTO.Requests
{
    public record UpdatedBoard
    {
        int BoardId { get; set; }
        public string UpdatedName { get; set; }
    }
}
