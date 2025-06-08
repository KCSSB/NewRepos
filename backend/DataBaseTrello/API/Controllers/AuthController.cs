using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataBaseInfo.Services;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using API.Requests;
using Microsoft.Extensions.Options;
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
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var  tokens = _userService.Login(loginRequest.UserEmail, loginRequest.UserPassword);
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
        [HttpPost("GetRefreshToken")]
        public IActionResult GetRefreshTokenbyId(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
   //             var userWithToken = await context.Users
     //.Include(u => u.RefreshToken) // Явно подгружаем RefreshToken
     //.FirstOrDefaultAsync(u => u.Id == id);

                return Ok(Request.Cookies["refreshToken"]);
            }
        }
    }
}
