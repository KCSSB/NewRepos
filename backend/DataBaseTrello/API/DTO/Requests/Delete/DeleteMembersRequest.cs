namespace API.DTO.Requests.Delete
{
    public record DeleteMembersRequest
    {
       public List<int> Members { get; set; } = new();
    }
}
