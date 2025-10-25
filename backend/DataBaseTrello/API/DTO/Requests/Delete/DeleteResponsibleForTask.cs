namespace API.DTO.Requests.Delete
{
    public record DeleteResponsibleForTask
    {
        public int TaskId { get; set; }
        public int MemberId { get; set; }
    }
}
