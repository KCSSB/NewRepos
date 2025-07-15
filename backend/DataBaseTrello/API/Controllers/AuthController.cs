using Microsoft.AspNetCore.Mvc;
using DataBaseInfo.Services;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo;
using API.Requests;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using API.Helpers;
using API.Configuration;
using System.Data.Common;
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
            try
            {

           var user = await _userService.RegisterAsync(request.UserEmail, request.UserPassword); //check parametrs
           using(var context = _contextFactory.CreateDbContext()) //Fix it
            {
                context.Users.Add(user);
               await context.SaveChangesAsync();


                user.UserName = $"user{user.Id:D6}";

                await context.SaveChangesAsync();
            }
                return Ok("User registered successfully");
            }
            catch (Exception)
            {
                //Ошибка при создании пользователя
                
                return BadRequest();
            }
            catch (InvalidCastException ex) //Ошибка при хэшировании пароля
            {
                //Логирование ошибки
                return BadRequest();
            }
            catch (Exception ex) //Ошибка существования идентичного Email
            {
                //Логирование ошибки
                return BadRequest();
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
            var  tokens = await _userService.LoginAsync(loginRequest.UserEmail, loginRequest.UserPassword);
            
            
                
            
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
            catch (InvalidCastException)
            {
                //Логирование ошибки Пользователь не найден
                return BadRequest();
            }
            catch (DbException)
            {
                //Логирование ошибки Вы уже были авторизованы
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                //Логирование ошибки Неверный пароль
                return BadRequest();
            }
            catch (Exception)
            {
                //Логирование ошибки Ошибка при получении пользователя(переданная из вложенного)
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
                //Логирование ошибки UpdateDbContext
            }
        }
        
        [HttpPost("RefreshAccessToken")]
        public async Task<IActionResult> RefreshAcessToken()
        {
            try
            {

                var refreshToken = Request.Cookies["refreshToken"];
            var tokens = await _jwtServices.RefreshTokenAsync(refreshToken);
            Response.Cookies.Append("refreshToken", (tokens.refreshToken), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.Add(_options.Value.RefreshTokenExpires)
            });
            return Ok(tokens.accessToken);
            }
            catch (Exception)
            {

                //Ошибка при получении User
                throw;
            }
            catch (Exception)
            {
                //Рефреш токен не действителен

                throw;
            }
            catch (Exception)
            {

                //рефреш токен отсутствует
                throw;
            }
        }

        [HttpDelete("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {

                var refreshToken = Request.Cookies["refreshToken"];
                bool flag = await _jwtServices.RevokeRefreshTokenAsync(refreshToken);
               
                
                    Response.Cookies.Delete("refreshToken");
                    return Ok("User success unauthorized");
            }
            catch (Exception)
            {
                //Рефреш токен отсутствует
                throw;
            }
            catch (Exception)
            {
                //Рефреш токен отсутствует в бд
                throw;
            }
        }

   
    }
}
