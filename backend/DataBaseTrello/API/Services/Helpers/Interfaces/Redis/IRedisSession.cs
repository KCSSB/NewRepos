using API.DTO.Domain;

namespace API.Services.Helpers.Interfaces.Redis
{
    public interface IRedisSession
    {
        Task SafeSetSessionAsync(string refreshToken, int userId, string deviceId);
        Task<SessionData?> SafeGetSessionAsync(int userId, string deviceId);
        Task<bool?> SafeRevokeSessionAsync(int userId, string deviceId);
    }
}
