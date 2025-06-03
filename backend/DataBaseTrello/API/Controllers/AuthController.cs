using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataBaseInfo.Services;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using static System.Runtime.InteropServices.JavaScript.JSType;
using API.Requests;
using Microsoft.AspNetCore.Identity;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        private readonly UserService _userService;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly JWTServices _jwtServices;

       
        public AuthController(UserService userService, IDbContextFactory<AppDbContext> contextFactory, JWTServices jwtServices)
        {
            _userService = userService;
            _contextFactory = contextFactory;
            _jwtServices = jwtServices;
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
            var Token = _userService.Login(loginRequest.UserEmail, loginRequest.UserPassword);
            return Ok(Token);
        }
        [HttpPost("GetRefreshToken")]
        public async Task<IActionResult> GetRefreshTokenbyId(int id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var userWithToken = await context.Users
     .Include(u => u.RefreshToken) // Явно подгружаем RefreshToken
     .FirstOrDefaultAsync(u => u.Id == id);

                return Ok(userWithToken.RefreshToken.Id);
            }
        }
    }
}
