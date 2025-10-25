namespace API.DTO.Requests.Change
{
    public record ChangedSubTask
    {
        public int SubTaskId { get; set; } 
        public string SubTaskName { get; set; }
    }
}
