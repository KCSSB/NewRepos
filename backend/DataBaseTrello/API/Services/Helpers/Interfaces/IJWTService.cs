using DataBaseInfo;
using DataBaseInfo.models;

namespace API.Services.Helpers.Interfaces
{
    public interface IJWTService
    {
        Task<string> CreateRefreshTokenAsync(User user, string? deviceId, AppDbContext? context = null);
        string GenerateAccessToken(User user, string? deviceId);
        Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken, int userId, string deviceId);
        void RefTokenIsValid(Session? token);
        Task RevokeSessionAsync(int userId, string deviceId);
        Task RevokeSessionsAsync(List<Session> sessions, Guid deviceId, int limit = 3);
    }
}