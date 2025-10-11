namespace API.DTO.Requests.Change
{
    public record ChangedCards
    {
        public int CardId { get; set; }
        public string CardName { get; set; }
    }
}
