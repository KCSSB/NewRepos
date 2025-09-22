namespace API.DTO.Requests
{
    public class DeleteProjectUsersRequest
    {
        public List<int> ProjectUsers { get; set; } = new();
    }
}
