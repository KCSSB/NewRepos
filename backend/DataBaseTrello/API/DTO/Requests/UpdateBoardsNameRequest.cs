namespace API.DTO.Requests
{
    public record UpdateBoardsNameRequest
    {
        public List<UpdatedBoard> UpdatedBoard { get; set; } = new();
    }
}
