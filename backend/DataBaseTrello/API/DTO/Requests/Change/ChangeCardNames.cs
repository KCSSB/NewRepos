namespace API.DTO.Requests.Change
{
    public record ChangeCardNames
    {
        public List<ChangedCards> Cards { get; set; } = new();
    }
}
