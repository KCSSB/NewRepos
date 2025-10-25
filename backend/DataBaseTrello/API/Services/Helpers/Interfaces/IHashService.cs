namespace API.Services.Helpers.Interfaces
{
    public interface IHashService
    {
        string HashToken(string token);
        bool VerifyToken(string token, string hashedToken);
    }
}