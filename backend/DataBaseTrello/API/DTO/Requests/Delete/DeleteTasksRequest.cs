namespace API.DTO.Requests.Delete
{
    public record DeleteTasksRequest
    {
        public List<int> TasksIds { get; set; }
    }
}
