namespace API.DTO.Responses.Pages.WorkSpacePage
{
    public record WorkSpaceTask
    {
        public int TaskId {  get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public int? ProgressBar {  get; set; }
        public string Priority { get; set; }
        public DateOnly? DateOfStartWork {  get; set; }
        public DateOnly? DateOfDeadline { get; set; }
        public List<WorkSpaceSubTask> SubTasks { get; set; } = new();
        public List<int> ResponsibleIds { get; set; } = new();
    }
}
