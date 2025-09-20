namespace API.DTO.Responses.Pages.HallPage
{
    public record HallBoard
    {
        public int BoardId { get; set; }
        public string BoardName { get; set; }
        public int ProgressBar {  get; set; }
        public int MembersCount { get; set; }
        public DateOnly? DateOfStartWork { get; set; }
        public DateOnly? DateOfDeadline {  get; set; }
        public int BoardLeadId { get; set; }

    }
}
