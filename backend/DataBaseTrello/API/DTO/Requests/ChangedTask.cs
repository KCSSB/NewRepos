namespace API.DTO.Requests
{
    public record ChangedTask
    {
        public int TaskId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Priority { get; set; }
        public DateOnly? DateOfStartWork { get; set; }
        public DateOnly? DateOfDeadline { get; set; }

    }
}
