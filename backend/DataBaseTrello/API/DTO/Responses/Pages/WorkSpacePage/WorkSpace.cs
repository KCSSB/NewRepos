namespace API.DTO.Responses.Pages.WorkSpacePage
{
    public record WorkSpace
    {
        public int ProjectId { get; set; }
        public int BoardId { get; set; }
        public string BoardName { get; set; }
        public string ProjectName { get; set; }
        public List<WorkSpaceCard> Cards { get; set; } = new();
        public List<WorkSpaceMember> Members { get; set; } = new();
        public string BoardRole { get; set; }
    }
}
