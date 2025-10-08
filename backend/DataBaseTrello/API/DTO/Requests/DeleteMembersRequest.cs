namespace API.DTO.Requests
{
    public record DeleteMembersRequest
    {
        List<int> Members { get; set; } = new();
    }
}
