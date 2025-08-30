
using System.Net;

using API.Constants;
using API.DTO.Domain;
using API.DTO.Mappers.ToResponseModel;
using API.DTO.Responses;
using API.Exceptions;
using API.Exceptions.Context;
using API.Extensions;
using API.Helpers;
using DataBaseInfo.models;

using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;


namespace DataBaseInfo.Services
{
    public class UserService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly JWTServices _JWTService;
        private readonly ErrorContextCreator _errCreator;
        private readonly ILogger<UserService> _logger;
        public UserService(IDbContextFactory<AppDbContext> contextFactory, JWTServices JWTService, ILogger<UserService> logger)
        {
            _contextFactory = contextFactory;
            _JWTService = JWTService;
            _logger = logger;
            _errCreator = new ErrorContextCreator(ServiceName.UserService);
        }
        public async Task<int> RegisterAsync(string userEmail, string password)
        {
            
            using var context = _contextFactory.CreateDbContext();
                
                    User? em = await context.Users.FirstOrDefaultAsync(em => em.UserEmail == userEmail);

            if (em != null)
                throw new AppException(_errCreator.Conflict($"Конфликт уникальности поля email: {userEmail}"));


                var user = new User
                    {
                        UserEmail = userEmail,
                        Avatar = DefaultImages.UserAvatar,
                        InviteId = Guid.NewGuid(),
                        Sex = Sex.Unknown
                    };

                    var passHash = new PasswordHasher<User>().HashPassword(user, password);

                    if (passHash == null)
                throw new AppException(_errCreator.Conflict("Ошибка во время хэширования пароля"));
            

                    user.UserPassword = passHash;
                
                    context.Users.Add(user);
                    //Возможны проблемы
                    await context.SaveChangesWithContextAsync("Ошибка во время сохранения данных о user в базу данных");
                    //Возможны проблемы
                return user.Id;

                }
        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(string UserEmail, string Password)
        {


            using var context = _contextFactory.CreateDbContext();
         
            User? user = await context.Users.FirstOrDefaultAsync(u => u.UserEmail == UserEmail);
                    if (user == null)
                    throw new AppException(_errCreator.Unauthorized($"Учётной записи с Email: {UserEmail} не существует"));
        
            var activeToken = await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => 
                rt.User.UserEmail == UserEmail
                && !rt.IsRevoked
                && rt.ExpiresAt > DateTime.UtcNow);

                    if (activeToken != null)
                    throw new AppException(_errCreator.Conflict($"Пользователь id: {user.Id}, email: {UserEmail}. Уже был авторизован"));
          
            var result = new PasswordHasher<User?>().VerifyHashedPassword(user, user.UserPassword, Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        var refreshToken = Guid.NewGuid().ToString();
             
                        await _JWTService.CreateRefreshTokenAsync(user, refreshToken);
                        var accessToken = _JWTService.GenerateAccessToken(user);
                        return (accessToken, refreshToken);

                    }
                    else
                    {
                    throw new AppException(_errCreator.Unauthorized($"Неверно введён пароль к аккаунту id: {user.Id}, email:{UserEmail}"));
                }
            
        }

        public async Task<string> UpdateUserAvatarAsync(Result? result, int userId)
        {
            if (result.HttpStatusCode >= 200 && result.HttpStatusCode < 300)
            {
                using var context = await _contextFactory.CreateDbContextAsync();

                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    throw new AppException(_errCreator.NotFound($"Произошла ошибка в момент смены аватара пользователя, Пользователь id: {userId}, не найден"));

                user.Avatar = result.url;
                await context.SaveChangesWithContextAsync($"Произошла ошибка в момент смены аватара пользователя, не удалось сохранить url: {result.url}");

                return result.url;
            }
            else
            {
                throw new AppException(_errCreator.InternalServerError($"Ошибка при загрузке изображения в ImageKit. Код: {result.HttpStatusCode}"));
            }
        }
        public async Task<UpdateUserModel> UpdateUserAsync(UpdateUserModel model, int userId)
        {
            var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new AppException(_errCreator.NotFound($"Ошибка при получении данных, user {userId} не найден"));
            
            if(!string.IsNullOrEmpty(model.FirstUserName))
                user.FirstName = model.FirstUserName;
            if (!string.IsNullOrEmpty(model.LastUserName))
                user.SecondName = model.LastUserName;
            if (model.Sex!=null)
                user.Sex = (Sex)model.Sex;

            await context.SaveChangesWithContextAsync($"Ошибка при обновлении данных о пользователе: {userId}");

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
            var context = await _contextFactory.CreateDbContextAsync();

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new AppException(_errCreator.NotFound($"Ошибка при получении данных, user {userId} не найден"));
            var passHasher = new PasswordHasher<User>();
            var result = passHasher.VerifyHashedPassword(user, user.UserPassword,oldPass);
            if(result == PasswordVerificationResult.Success)
            {
                var newPassHash = passHasher.HashPassword(user, newPass);
                user.UserPassword = newPassHash;
                await context.SaveChangesWithContextAsync($"Ошибка при обновлении пароля в базе данных: {userId}");
               
            }
            else
            {

                throw new AppException(_errCreator.BadRequest($"Изменение не удалось, неверный старый пароль userId{userId}"));
            }

        }

    } 
        
    
}
