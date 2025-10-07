namespace API.DTO.Responses.Pages.WorkSpacePage
{
    public record WorkSpaceSubTask
    {
        public string SubTaskId { get; set; }
        public string SubTaskName { get; set; }
        public bool IsCompleted { get; set; }
    }
}
