namespace API.DTO.Requests
{
    public record DeleteBoardsRequest
    {
        List<int> BoardIds { get; set; } = new();
    }
}
