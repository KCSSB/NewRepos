using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using API.Configuration;
using API.Constants;
using API.Exceptions.ErrorContext;
using API.Extensions;
using API.Helpers;
using DataBaseInfo.models;
using Imagekit.Sdk;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DataBaseInfo.Services
{
    public class UserService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly JWTServices _JWTService;
        private readonly ImagekitClient _imagekitClient;
        private readonly ImageKitSettings _settings;
        private readonly ILogger<UserService> _logger;
        public UserService(IDbContextFactory<AppDbContext> contextFactory, JWTServices JWTService, IOptions<ImageKitSettings> options, ILogger<UserService> logger)
        {
            _contextFactory = contextFactory;
            _settings = options.Value;
            _JWTService = JWTService;
            _imagekitClient = new ImagekitClient(publicKey: _settings.PublicKey,
                privateKey: _settings.PrivateKey,
                urlEndPoint: _settings.UrlEndpoint);
            _logger = logger;
        }
        public async Task<string> UploadUserAvatarAsync(IFormFile file, Guid userId)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            var base64 = Convert.ToBase64String(fileBytes);

            var request = new FileCreateRequest
            {
                file = $"data:{file.ContentType};base64,{base64}",
                fileName = file.FileName,
                useUniqueFileName = true,
                folder = "/UsersAvatars"
            };
            var result = await _imagekitClient.UploadAsync(request);
            if (result.HttpStatusCode >= 200 && result.HttpStatusCode < 300)
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    throw new AppException(new ErrorContext(ServiceName.UserService,
                 OperationName.UploadUserAvatarAsync,
                 HttpStatusCode.InternalServerError,
                UserExceptionMessages.UploadFilesExceptionMessage,
                $"Произошла ошибка в момент смены аватара пользователя, Пользователь id: {userId}, не найден"));
                

                user.Avatar = result.url;
                await context.SaveChangesWithContextAsync(ServiceName.UserService,
                    OperationName.UploadUserAvatarAsync,
                    $"Произошла ошибка в момент смены аватара пользователя, не удалось сохранить url: {result.url}",
                    UserExceptionMessages.UploadFilesExceptionMessage,
                    HttpStatusCode.InternalServerError);

                return result.url;
            }
            else
            {
                throw new AppException(new ErrorContext(
            ServiceName.UserService,
            OperationName.UploadUserAvatarAsync,
            (HttpStatusCode)result.HttpStatusCode,
            UserExceptionMessages.UploadFilesExceptionMessage,
            $"Ошибка при загрузке изображения в ImageKit. Код: {result.HttpStatusCode}"));
            }
           }
        public async Task<Guid> RegisterAsync(string userEmail, string password)
        {

            using var context = _contextFactory.CreateDbContext();
                
                    User? em = await context.Users.FirstOrDefaultAsync(em => em.UserEmail == userEmail);

                    if (em != null)
                        throw new AppException(new ErrorContext(ServiceName.UserService,
                        OperationName.RegisterAsync,
                        HttpStatusCode.Conflict,
                        "Пользователь с таким email уже существует",
                        $"Конфликт уникальности поля email: {userEmail}")); 


                var user = new User
                    {
                        UserEmail = userEmail,
                        Avatar = DefaultImages.UserAvatar,
                        InviteId = Guid.NewGuid(),
                        Sex = Sex.Unknown
                    };

                    var passHash = new PasswordHasher<User>().HashPassword(user, password);

                    if (passHash == null)
                    throw new AppException(new ErrorContext(ServiceName.UserService,
                        OperationName.RegisterAsync,
                        HttpStatusCode.InternalServerError,
                        UserExceptionMessages.RegistrationExceptionMessage,
                        "Ошибка во время хэширования пароля"));

                    user.UserPassword = passHash;
                
                    context.Users.Add(user);
                    //Возможны проблемы
                    await context.SaveChangesWithContextAsync(ServiceName.UserService,
                        OperationName.RegisterAsync,
                        "Ошибка во время сохранения данных о user в базу данных",
                        UserExceptionMessages.RegistrationExceptionMessage,
                        HttpStatusCode.InternalServerError);
                    //Возможны проблемы
                return user.Id;

                }
            
            
            


        

        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(string UserEmail, string Password)
        {


            using var context = _contextFactory.CreateDbContext();
         
            User? user = await context.Users.FirstOrDefaultAsync(u => u.UserEmail == UserEmail);
                    if (user == null)
                    throw new AppException(new ErrorContext(ServiceName.UserService,
                    OperationName.LoginAsync,
                    HttpStatusCode.Unauthorized,
               "Неправильный логин или пароль",
               $"Учётной записи с Email: {UserEmail} не существует"));
        
            var activeToken = await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.User.UserEmail == UserEmail
                                   && !rt.IsRevoked
                                   && rt.ExpiresAt > DateTime.UtcNow);
                    if (activeToken != null)
                    throw new AppException(new ErrorContext(ServiceName.UserService,
                OperationName.LoginAsync,
                HttpStatusCode.BadRequest,
           "Вы уже были авторизованы",
           $"Пользователь id: {user.Id}, email: {UserEmail}. Уже был авторизован"));
          
            var result = new PasswordHasher<User?>().VerifyHashedPassword(user, user.UserPassword, Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        var token = Guid.NewGuid().ToString();
             
                await _JWTService.CreateRefreshTokenAsync(user, token);
          
                return (_JWTService.GenerateAccessToken(user), token);

                    }
                    else
                    {
                    throw new AppException(new ErrorContext(ServiceName.UserService,
                OperationName.LoginAsync,
                HttpStatusCode.Unauthorized,
           "Неправильный логин или пароль",
           $"Неверно введён пароль к аккаунту id: {user.Id}, email:{UserEmail} "));
                }
            
        }
            
        
    } 
        
    
}
