using DataBaseInfo;
using DataBaseInfo.models;

namespace API.Services.Helpers.Interfaces
{
    public interface IJWTService
    {
        Task<(string token, string deviceId)> CreateRefreshTokenAsync(User user, string? deviceId);
        string GenerateAccessToken(User user, string? deviceId);
        Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken, string deviceId);
        void RefTokenIsValid(Session? token);
        Task RevokeSessionAsync(int userId, string deviceId, string token);
        Task RevokeSessionsAsync(List<Session> sessions, string? deviceId, int limit = 3);
    }
}