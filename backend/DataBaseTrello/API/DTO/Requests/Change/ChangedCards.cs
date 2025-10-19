namespace API.DTO.Requests.Change
{
    public record ChangedCard
    {
        public int CardId { get; set; }
        public string CardName { get; set; }
    }
}
