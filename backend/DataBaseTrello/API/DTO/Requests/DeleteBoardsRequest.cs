namespace API.DTO.Requests
{
    public record DeleteBoardsRequest
    {
        public List<int> BoardIds { get; set; } = new();
    }
}
