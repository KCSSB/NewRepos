﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using API.Configuration;
using API.Exceptions.Context;
using API.Constants;
using API.Extensions;
using API.Exceptions.ContextCreator;
using API.Services.Application.Interfaces;
using API.Services.Helpers.Interfaces;
using API.DTO.Requests.Create;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJWTService _jwtServices;
        private readonly IOptions<AuthSettings> _options;
        private readonly ILogger<AuthController> _logger;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;
        public AuthController(IUserService userService, IJWTService jwtServices, IOptions<AuthSettings> options, ILogger<AuthController>  logger, IErrorContextCreatorFactory errCreatorFactory)
        {
            _errCreatorFactory = errCreatorFactory;
            _userService = userService;
            _jwtServices = jwtServices;
            _options = options;
            _logger = logger;
        }
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(AuthController));

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserRequest request)//Refactor
        {

            if (request == null)
                return BadRequest();

            if (!ModelState.IsValid)
                throw new AppException(_errCreator.BadRequest($"Данные переданные в экземпляр RegisterUserRequest не валидны {request.UserEmail}"));

            int userId = await _userService.RegisterAsync(request.UserEmail, request.UserPassword);
        
            _logger.LogInformation(InfoMessages.FinishOperation + "Register");

            return Ok(new{ id = userId });

        }

        [HttpPost("login")] //Refactoring
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null)
                return BadRequest();

            if (!ModelState.IsValid)
                throw new AppException(_errCreator.BadRequest("Данные переданные в экземпляр loginRequest не валидны"));
            
            string? deviceId = User.GetDeviceId();
            var tokens = await _userService.LoginAsync(loginRequest.UserEmail, loginRequest.UserPassword, deviceId);
       
            Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.Add(_options.Value.RefreshTokenExpires)
            });
          
            _logger.LogInformation(InfoMessages.FinishOperation + "Login");

            return Ok(new { accessToken = tokens.AccessToken });
            
        }
        
        [HttpPost("RefreshAccessToken")]
        public async Task<IActionResult> RefreshAccessToken()
        {
            
            _logger.LogInformation(InfoMessages.StartOperation + "RefreshAccessToken");
            string? deviceId = User.GetDeviceId();
           
            var refreshToken = Request.Cookies["refreshToken"]; 

            if (refreshToken == null)
                throw new AppException(_errCreator.Unauthorized("Произошла ошибка во время получения RefreshToken из Cookies"));
            var tokens = await _jwtServices.RefreshTokenAsync(refreshToken, deviceId);
            
            Response.Cookies.Append("refreshToken", (tokens.refreshToken), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.Add(_options.Value.RefreshTokenExpires)
            });
            _logger.LogInformation(InfoMessages.FinishOperation + "RefreshAccessToken");
            return Ok(new { accessToken = tokens.accessToken });

        }
            [Authorize]
            [HttpDelete("Logout")]
            public async Task<IActionResult> Logout()
            {
            _logger.LogInformation(InfoMessages.StartOperation + "Logaut");
            string token = Request.Cookies["refreshToken"];
            int userId = User.GetUserId();
            string? deviceId = User.GetDeviceId();
            
            await _jwtServices.RevokeSessionAsync(userId, deviceId, token);
 
            _logger.LogInformation(InfoMessages.FinishOperation + "RefreshAccessToken");
            return Ok("User success unauthorized");

            }

    }
}
