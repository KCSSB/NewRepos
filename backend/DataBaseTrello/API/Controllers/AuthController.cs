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
using API.Exceptions.Enumes;
using System.Net;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly UserService _userService;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly JWTServices _jwtServices;
        private readonly IOptions<AuthSettings> _options;
        
      

       
        public AuthController(UserService userService, IDbContextFactory<AppDbContext> contextFactory, JWTServices jwtServices, IOptions<AuthSettings> options)
        {
            _userService = userService;
            _contextFactory = contextFactory;
            _jwtServices = jwtServices;
            _options = options;
          
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserRequest request)
        {
           //Возможны проблемы
           var user = await _userService.RegisterAsync(request.UserEmail, request.UserPassword);
            //Возможны проблемы

            using (var context = _contextFactory.CreateDbContext()) 
            {

                context.Users.Add(user);
                //Возможны проблемы
                await context.SaveChangesAsync();
                //Возможны проблемы

                user.UserName = $"user{user.Id:D6}";
                //Возможны проблемы
                await context.SaveChangesAsync();
                //Возможны проблемы
            }
            return Ok("User registered successfully");

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            //Возможны проблемы
            var tokens = await _userService.LoginAsync(loginRequest.UserEmail, loginRequest.UserPassword);
            //Возможны проблемы



            var accessToken = tokens?.AccessToken;
            var refreshToken = tokens?.RefreshToken;
            Response.Cookies.Append("refreshToken", (refreshToken.ToString()), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.Add(_options.Value.RefreshTokenExpires)
            });
            
            return Ok(accessToken);
            
        }

        [HttpPost("RefreshAccessToken")]
        public async Task<IActionResult> RefreshAccessToken()
        {
            //Валидация здесь
            
            var refreshToken = Request.Cookies["refreshToken"]; //Кастомная ошибка потери refreshToken
            if (refreshToken == null)
                throw new AppException(new ErrorContext(ServiceName.AuthController,
                    OperationName.RefreshAccessToken,
                    HttpStatusCode.Unauthorized,
                    "Ошибка авторизации",
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
            return Ok(tokens.accessToken);

        }
            [HttpDelete("Logout")]
            public async Task<IActionResult> Logout()
            {
            //Валдиация Здесь
                var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken.IsNullOrEmpty())
                throw new Exception();
            //Валидация Здесь
            //Возможны проблемы
                bool flag = await _jwtServices.RevokeRefreshTokenAsync(refreshToken);
            //Возможно проблемы

                Response.Cookies.Delete("refreshToken");
                return Ok("User success unauthorized");

            }
        

   
    }
}
