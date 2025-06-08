using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataBaseInfo.Services;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using API.Requests;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
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
           var user = _userService.Register(request.UserEmail, request.UserPassword);
           using(var context = _contextFactory.CreateDbContext())
            {
                context.Users.Add(user);
               await context.SaveChangesAsync();


                user.UserName = $"user{user.Id:D6}";

                await context.SaveChangesAsync();
            }
                return Ok("User registered successfully");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var  tokens = await _userService.Login(loginRequest.UserEmail, loginRequest.UserPassword);
            var accessToken = tokens.AcessToken;
            var refreshToken = tokens.RefreshToken;
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
        public async Task<IActionResult> RefreshAcessToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Refresh token is missing.");

            var tokens = await _jwtServices.RefreshTokenAsync(refreshToken);
            if (tokens.accessToken == null || tokens.refreshToken == null)
                return Unauthorized("Invalid or expired refresh token.");
            Response.Cookies.Append("refreshToken", (tokens.refreshToken), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.Add(_options.Value.RefreshTokenExpires)
            });
            return Ok(tokens.accessToken);
        }
        [HttpPost("GetRefreshTokenFromCookie")]
        public IActionResult GetRefreshToken(int Id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var RefreshToken = context.RefreshTokens.FirstOrDefault(rt => rt.User.Id == Id);
                var Token = RefreshToken.Token;
                return Ok(Token); 
            }
        }
    }
}
