namespace API.DTO.Requests
{
    public record ChangeCardNames
    {
        public List<ChangedCards> Cards { get; set; } = new();
    }
}
