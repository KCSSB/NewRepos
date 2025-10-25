namespace API.DTO.Responses.Pages.HallPage
{
    public record HallPage
    {
        public int ProjectId {  get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public List<HallProjectUser> ProjectUsers { get; set; } = new();
        public List<HallBoard> Boards { get; set; } = new();
        public string ProjectRole { get; set; }
    }
}
