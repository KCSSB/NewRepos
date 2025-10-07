namespace API.DTO.Requests
{
    public record TasksChangeRequest
    {
        public List<ChangedTask> tasks { get; set; } = new();
    }
}
