namespace API.DTO.Requests
{
    public record ChangedSubTasksRequest
    {
        public List<ChangedSubTask> subTasks { get; set; } = new();
    }
}
