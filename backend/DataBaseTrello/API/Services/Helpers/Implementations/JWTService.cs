using System.Text;
using DataBaseInfo.models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using API.Configuration;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Services.Helpers.Interfaces;
using API.Repositories.Uof;
using API.Repositories.Queries;
namespace API.Services.Helpers.Implementations
{
    public class JWTService : IJWTService
    {
        private readonly string ServiceName = nameof(JWTService);
        private readonly IOptions<AuthSettings> _options;
        private readonly IHashService _hashService;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueries _query;
        private ErrorContextCreator? _errorContextCreator;


        public JWTService(IOptions<AuthSettings> options,
            IHashService hashService,
            ILogger<IJWTService> logger,
            IErrorContextCreatorFactory errCreatorFactory,
            IUnitOfWork unitOfWork,
            IQueries query)
        {
            _errCreatorFactory = errCreatorFactory;
            _hashService = hashService;
            _options = options;
            _unitOfWork = unitOfWork;
            _query = query;
        }
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(IJWTService));
        public string GenerateAccessToken(User user, string? deviceId)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim("FirstName", user.FirstName),
                new Claim("SecondName", user.SecondName),
               new Claim("Sex", SexHelper.GetSexDisplay(user.Sex)),
               new Claim("InviteId", user.InviteId.ToString()),
                new Claim("UserEmail", user.UserEmail),
                new Claim("Avatar", user.Avatar),
                new Claim("DeviceId", deviceId.ToString()),
            };

            var jwtToken = new JwtSecurityToken(
                expires: DateTime.UtcNow.Add(_options.Value.AccessTokenExpires),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey)), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public async Task<(string token, string deviceId)> CreateRefreshTokenAsync(User user, string? deviceId)
        {
            var token = Guid.NewGuid().ToString();
            if (deviceId == null)
                deviceId = Guid.NewGuid().ToString();
            var hashedToken = _hashService.HashToken(token);
            var session = new Session
            {
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(_options.Value.RefreshTokenExpires),
                Token = hashedToken,
                IsRevoked = false,
                UserId = user.Id,
                DeviceId = Guid.Parse(deviceId),
            };
            user.Sessions.Add(session);

            await _unitOfWork.SessionRepository.AddDbSessionAsync(session);

            await _unitOfWork.SaveChangesAsync(ServiceName, "Ошибка при сохранении Hashed Refresh Token");

            return (token,deviceId);
        }


        public async Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken, string deviceId)
        {
          

            var hashToken = _hashService.HashToken(refreshToken);

            var curSession = await _query.SessionQueries.GetCurSessionWithUser(hashToken);

            RefTokenIsValid(curSession);
            curSession.IsRevoked = true;

            var res = await CreateRefreshTokenAsync(curSession.User, deviceId);
            var newAccessToken = GenerateAccessToken(curSession.User, res.deviceId);

            var token = res.token;
            return (newAccessToken, token);
        }

        public void RefTokenIsValid(Session? token)
        {
            if (token == null)
                throw new AppException(_errCreator.Unauthorized("RefreshToken не существует"));
            else if (token.IsRevoked)
                throw new AppException(_errCreator.Unauthorized("RefreshToken был отозван"));
            else if (token.ExpiresAt < DateTime.UtcNow)
                throw new AppException(_errCreator.Unauthorized("RefreshToken истёк"));
        }
        public async Task RevokeSessionAsync(int userId, string deviceId, string token)
        {
         

            var session = await _unitOfWork.SessionRepository.GetDbSessionAsync(userId, deviceId, token);

            if (session == null)
                throw new AppException(_errCreator.Unauthorized("В базе не найдена сессия соответствующий значению из cookie при попытке выйти из аккаунта"));
            session.IsRevoked = true;
            await _unitOfWork.SaveChangesAsync(ServiceName, $"Ошибка при отзыве сессии из базы данных session id: {session.Id}");

        }
        private async Task RevokeSessionRangeAsync(List<Session> sessions)
        {
            if (sessions.Count == 0)
                return;
            foreach (var session in sessions)
                session.IsRevoked = true;
            await _unitOfWork.SaveChangesAsync(ServiceName, $"Ошибка при удалении сессий из бд");
        }
        private async Task RevokeSessionToTheLimitAsync(List<Session> sessions, List<Session> forRevoke, bool hasDevice = false, int limit = 3)
        {
            if (sessions.Count < limit)
                return;
            if (hasDevice == true)
                forRevoke.AddRange(sessions.Skip(limit + 1));
            else
                forRevoke.AddRange(sessions.Skip(limit));
            await RevokeSessionRangeAsync(forRevoke);

        }
        private bool RevokeThisDeviceSessions(List<Session> sessions, List<Session> forRevoke, Guid? deviceId, int limit = 3)
        {
            bool hasDeviceId = false;
            if (deviceId == null) return hasDeviceId;
            int i = 0;
            foreach (var session in sessions)
            {
                if (i > 3)
                    return hasDeviceId;
                if (session.DeviceId == deviceId)
                {
                    forRevoke.Add(session);
                    hasDeviceId = true;
                }
                i++;
            }

            return hasDeviceId;
        }
        public async Task RevokeSessionsAsync(List<Session> sessions, string? deviceId, int limit = 3)
        {
            Guid? deviceIdG = null;
            if (deviceId != null)
                deviceIdG = Guid.Parse(deviceId);
            List<Session> forRevoked = new List<Session>();
         
                bool hasDeviceId = RevokeThisDeviceSessions(sessions, forRevoked, deviceIdG, limit);

            if (sessions.Count > limit)
                await RevokeSessionToTheLimitAsync(sessions, forRevoked, hasDeviceId, limit);
        }
    }
}
