namespace API.DTO.Requests.Change
{
    public record ChangeCardNames
    {
        public List<ChangedCard> Cards { get; set; } = new();
    }
}
