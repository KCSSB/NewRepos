namespace API.DTO.Domain
{
    public class SessionData
    {
        public bool IsRevoked { get; set; }
        public string HashedToken { get; set; }
    }
}
