
namespace API.Services.Application.Interfaces
{
    public interface ISessionService
    {
        public Task<bool?> SessionIsRevokedAsync(int userId, string deviceId, string refreshToken);  
    }
}
