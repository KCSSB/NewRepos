using Microsoft.AspNetCore.Mvc;
using DataBaseInfo.Services;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using API.Helpers;
using API.Configuration;
using System.Data.Common;
using API.DTO.Requests;
using Microsoft.IdentityModel.Tokens;
using API.Exceptions;
using API.Exceptions.ErrorContext;
using System.Net;
using API.Extensions;
using API.Constants;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly UserService _userService;
        private readonly JWTServices _jwtServices;
        private readonly IOptions<AuthSettings> _options;
        private readonly ILogger<AuthController> _logger;
        
      

       
        public AuthController(UserService userService, JWTServices jwtServices, IOptions<AuthSettings> options, ILogger<AuthController>  logger)
        {
            _userService = userService;
            _jwtServices = jwtServices;
            _options = options;
            _logger = logger;
          
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserRequest request)
        {

            _logger.LogInformation(InfoMessages.StartOperation + OperationName.Register);

            if (!ModelState.IsValid)
                throw new AppException(new ErrorContext(ServiceName.AuthController,
                   OperationName.Register,
                   HttpStatusCode.BadRequest,
                   UserExceptionMessages.IncorrectDataExceptionMessage,
                   "Данные переданные в экземпляр RegisterUserRequest не валидны"));

            //Возможны проблемы
            Guid UserId = await _userService.RegisterAsync(request.UserEmail, request.UserPassword);
            //Возможны проблемы
            _logger.LogInformation(InfoMessages.FinishOperation + OperationName.Register);
            return Ok(new{ id = UserId }); //Возвращать URL

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            _logger.LogInformation(InfoMessages.StartOperation + OperationName.Login);
            if (!ModelState.IsValid)
                throw new AppException(new ErrorContext(ServiceName.AuthController,
                   OperationName.Login,
                   HttpStatusCode.BadRequest,
                   UserExceptionMessages.IncorrectDataExceptionMessage,
                   "Данные переданные в экземпляр loginRequest не валидны"));
          
            var tokens = await _userService.LoginAsync(loginRequest.UserEmail, loginRequest.UserPassword);
       
            Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.Add(_options.Value.RefreshTokenExpires)
            });
          
            _logger.LogInformation(InfoMessages.FinishOperation + OperationName.Login);
            return Ok(new { accessToken = tokens.AccessToken });
            
        }
        
        [HttpPost("RefreshAccessToken")]
        public async Task<IActionResult> RefreshAccessToken()
        {
            //Валидация здесь
            _logger.LogInformation(InfoMessages.StartOperation + OperationName.RefreshAccessToken);
            var refreshToken = Request.Cookies["refreshToken"]; //Кастомная ошибка потери refreshToken
            if (refreshToken == null)
                throw new AppException(new ErrorContext(ServiceName.AuthController,
                    OperationName.RefreshAccessToken,
                    HttpStatusCode.Unauthorized,
                    UserExceptionMessages.AuthorizeExceptionMessage,
                    "Произошла ошибка во время получения RefreshToken из Cookies"));
              
            //Валидация здесь
            //Возможно проблемы
            var tokens = await _jwtServices.RefreshTokenAsync(refreshToken);
            //Возможно проблемы
            Response.Cookies.Append("refreshToken", (tokens.refreshToken), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.Add(_options.Value.RefreshTokenExpires)
            });
            _logger.LogInformation(InfoMessages.FinishOperation + OperationName.RefreshAccessToken);
            return Ok(new { accessToken = tokens.accessToken });

        }
            [Authorize]
            [HttpDelete("Logout")]
            public async Task<IActionResult> Logout()
            {
            _logger.LogInformation(InfoMessages.StartOperation + OperationName.Logout);
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                throw new AppException(new ErrorContext(ServiceName.AuthController,
                    OperationName.Logout,
                    HttpStatusCode.Unauthorized,
                    UserExceptionMessages.AuthorizeExceptionMessage,
                    "Произошла ошибка во время получения RefreshToken из Cookies"));

            
            
            await _jwtServices.RevokeRefreshTokenAsync(refreshToken);
            
                Response.Cookies.Delete("refreshToken");
            _logger.LogInformation(InfoMessages.FinishOperation + OperationName.RefreshAccessToken);
            return Ok("User success unauthorized");

            }
        

   
    }
}
