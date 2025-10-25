namespace API.DTO.Responses.Pages.WorkSpacePage
{
    public record WorkSpaceCard
    {
        public int CardId { get; set; }
        public string CardName { get; set; }
        public List<WorkSpaceTask> Tasks { get; set; } = new();
        public DateOnly? DateOfStartWork {  get; set; }
        public DateOnly? DateOfDeadline {  get; set; }

    }
}
