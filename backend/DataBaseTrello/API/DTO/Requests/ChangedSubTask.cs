namespace API.DTO.Requests
{
    public record ChangedSubTask
    {
        public int SubTaskId { get; set; } 
        public string SubTaskName { get; set; }
    }
}
