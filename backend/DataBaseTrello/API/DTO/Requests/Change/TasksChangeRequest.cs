namespace API.DTO.Requests.Change
{
    public record TasksChangeRequest
    {
        public List<ChangedTask> tasks { get; set; } = new();
    }
}
