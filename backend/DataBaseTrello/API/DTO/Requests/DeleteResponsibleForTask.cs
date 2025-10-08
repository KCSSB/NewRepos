namespace API.DTO.Requests
{
    public record ResponsibleForTask
    {
        public int TaskId { get; set; }
        public int MemberId { get; set; }
    }
}
