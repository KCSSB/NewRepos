using API.Exceptions.ContextCreator;
using API.Repositories.Uof;
using API.Services.Application.Interfaces;
using API.Services.Helpers.Interfaces;


namespace API.Services.Application.Implementations
{
    public class SessionService : ISessionService
    {
        private readonly IHashService _hashService;
        private readonly IErrorContextCreatorFactory errorContextCreatorFactory;
        private ErrorContextCreator errCreator;
        private readonly IUnitOfWork _unitOfWork;
        public SessionService(IHashService hashService, 
            IErrorContextCreatorFactory _errorCreatorFactory,
            IUnitOfWork unitOfWork)
            {
            _hashService = hashService;
            _unitOfWork = unitOfWork;
            }
        private ErrorContextCreator _errCreator => errCreator ??= errorContextCreatorFactory.Create(nameof(ISessionService));
        public async Task<bool?> SessionIsRevokedAsync(int userId, string deviceId, string refreshToken)
        {
            var dbSessions = await _unitOfWork.SessionRepository.GetRangeSessionsAsync(userId, deviceId, refreshToken);

            var dbSession = dbSessions.FirstOrDefault(s => _hashService.VerifyToken(refreshToken, s.Token));

            if (dbSession != null) return dbSession.IsRevoked;
            return null;
        }
    }
}
