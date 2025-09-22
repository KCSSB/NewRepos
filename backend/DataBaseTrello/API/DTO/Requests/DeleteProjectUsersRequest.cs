namespace API.DTO.Requests
{
    public class DeleteProjectUsersRequest
    {
        List<int> ProjectUsers { get; set; } = new();
    }
}
