namespace API.DTO.Requests
{
    public record UpdateBoardsNameRequest
    {
        List<UpdatedBoard> UpdatedBoard { get; set; } = new();
    }
}
