namespace API.DTO.Requests.Change
{
    public record ChangedSubTasksRequest
    {
        public List<ChangedSubTask> subTasks { get; set; } = new();
    }
}
