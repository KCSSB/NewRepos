namespace API.DTO.Requests
{
    public record ChangedCards
    {
        public int CardId { get; set; }
        public string CardName { get; set; }
    }
}
