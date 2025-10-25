namespace API.DTO.Requests.Delete
{
    public record DeleteSubTasksRequest
    {
        public List<int> SubTasksIds { get; set; }
    }
}
