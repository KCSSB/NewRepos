using System.Security.Authentication.ExtendedProtection;
using API.DTO.Domain;
using API.DTO.Mappers;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Extensions;
using API.Repositories.Interfaces;
using API.Repositories.Queries;
using API.Repositories.Queries.Intefaces;
using API.Repositories.Uof;
using API.Services.Application.Interfaces;
using API.Services.Helpers.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace API.Services.Application.Implementations
{
    public class UserService : IUserService
    {
        private readonly string ServiceName = nameof(UserService);
        private readonly IJWTService _JWTService;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueries _query;
        private readonly IPasswordHasher<User> _passHasher;
        private ErrorContextCreator? _errorContextCreator;
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(ServiceName);

        public UserService(IJWTService IJWTService, 
            IErrorContextCreatorFactory errCreatorFactory,
            IUnitOfWork unitOfWork, 
            IQueries query
            , IPasswordHasher<User> passHasher)
        {
            _errCreatorFactory = errCreatorFactory;
            _JWTService = IJWTService;
            _unitOfWork = unitOfWork;
            _query = query;
            _passHasher = passHasher;
        }
        public async Task<int> RegisterAsync(string userEmail, string password)
        {
            User? user = await _unitOfWork.UserRepository.GetDbUserAsync(userEmail);
            if (user != null)
                throw new AppException(_errCreator.Conflict($"Пользователь с таким email: {userEmail}, уже существует"));
             var newUser = new User
             {
                UserEmail = userEmail,
                Avatar = DefaultImages.UserAvatar,
                InviteId = Guid.NewGuid(),
                Sex = Sex.Unknown
             };
            var passHash = _passHasher.HashPassword(newUser, password);
            if (passHash == null)
                throw new AppException(_errCreator.InternalServerError("Ошибка во время хэширования пароля"));
            newUser.UserPassword = passHash;
            await _unitOfWork.UserRepository.AddDbUserAsync(newUser);
            await _unitOfWork.SaveChangesAsync("Ошибка во время сохранения данных о user в базу данных", ServiceName);
            return newUser.Id;
        }
        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(string userEmail, string password, string? deviceId)
        {

            User? user = await _unitOfWork.UserRepository.GetDbUserAsync(userEmail);
            if (user == null)
                throw new AppException(_errCreator.NotFound($"Учётной записи с Email: {userEmail} не существует"));
            if (deviceId == null)
                deviceId = Guid.NewGuid().ToString();
           
            var result = _passHasher.VerifyHashedPassword(user, user.UserPassword, password);

            if (result != PasswordVerificationResult.Success)
                throw new AppException(_errCreator.Unauthorized($"Неверно введён пароль к аккаунту id: {user.Id}, email:{userEmail}"));

            var activeSessions = await _query.SessionQueries.GetActiveSessions(user.Id);
            if (activeSessions != null)
            {
            var sortedSessions = SortByDateCreateDesc(activeSessions);
            await _JWTService.RevokeSessionsAsync(sortedSessions, Guid.Parse(deviceId), 2);
            }

            var refreshToken = await _JWTService.CreateRefreshTokenAsync(user,deviceId);       
            var accessToken = _JWTService.GenerateAccessToken(user, deviceId);
            await _unitOfWork.SaveChangesAsync("Ошибка при попытке авторизации", ServiceName);
            return (accessToken, refreshToken);
        }

        public async Task<string> UpdateUserAvatarAsync(Result? result, int userId)
        {
            if (result.HttpStatusCode >= 400 && result.HttpStatusCode <= 500)
                throw new AppException(_errCreator.InternalServerError($"Ошибка при загрузке изображения в ImageKit. Код: {result.HttpStatusCode}"));
            var user = await _unitOfWork.UserRepository.GetDbUserAsync(userId);

            if (user == null)
                throw new AppException(_errCreator.NotFound($"Произошла ошибка в момент смены аватара пользователя, Пользователь id: {userId}, не найден"));

            user.Avatar = result.url;
            await _unitOfWork.SaveChangesAsync(ServiceName, $"Произошла ошибка в момент смены аватара пользователя, не удалось сохранить url: {result.url}");

            return result.url;
          
        }
        public async Task<UpdateUserModel> UpdateUserAsync(UpdateUserModel model, int userId)
        {
            var user = await _unitOfWork.UserRepository.GetDbUserAsync(userId);

            if (user == null)
                throw new AppException(_errCreator.NotFound($"Ошибка при получении данных, user {userId} не найден"));

            ToEntityMapper.ApplyToUser(model, user);

            await _unitOfWork.SaveChangesAsync(ServiceName, $"Ошибка при обновлении данных о пользователе: {userId}");

            var updatedUser = new UpdateUserModel
            {
                FirstUserName = user.FirstName,
                LastUserName = user.SecondName,
                Sex = user.Sex,
            };
            return updatedUser;
                                                                
        }
        public async Task ChangePasswordAsync(string oldPass, string newPass, int userId)
        {
            var user = await _unitOfWork.UserRepository.GetDbUserAsync(userId);
            if (user == null)
                throw new AppException(_errCreator.NotFound($"Ошибка при получении данных, user {userId} не найден"));
            var passHasher = new PasswordHasher<User>();
            var result = passHasher.VerifyHashedPassword(user, user.UserPassword,oldPass);
            if(result == PasswordVerificationResult.Success)
            {
                var newPassHash = passHasher.HashPassword(user, newPass);
                user.UserPassword = newPassHash;
                await _unitOfWork.SaveChangesAsync(ServiceName, $"Ошибка при обновлении пароля в базе данных: {userId}");
                // Добавить логику разлогинивания всех вообще кроме нынешней сессий
            }
            else
            {
                throw new AppException(_errCreator.BadRequest($"Изменение не удалось, неверный старый пароль userId{userId}"));
            }

        }
        private List<Session> SortByDateCreateDesc(List<Session> sessions)
        {
            var sortedList = sessions.OrderByDescending(s => s.CreatedAt).ToList();
            return sortedList;
        }

    } 
        
    
}
