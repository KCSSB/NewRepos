namespace API.DTO.Requests
{
    public class UpdateSubTaskRequest
    {
        public int SubTaskId { get; set; }
        public bool IsCompleted { get; set; }
    }
}
