namespace API.DTO.Requests
{
    public record UpdatedBoard
    {
        public int BoardId { get; set; }
        public string UpdatedName { get; set; }
    }
}
