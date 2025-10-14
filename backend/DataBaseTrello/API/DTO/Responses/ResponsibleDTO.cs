namespace API.DTO.Responses
{
    public record ResponsibleDTO
    {
        public int MemberId {  get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }

        public string AvatarUrl { get; set; }
    }
}
