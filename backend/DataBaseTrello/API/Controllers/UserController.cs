using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Imagekit;
using Imagekit.Sdk;
using Microsoft.Extensions.Options;
using API.Configuration;
using API.Constants;
using API.Exceptions.ErrorContext;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using API.Helpers;
using Microsoft.IdentityModel.Tokens;
using DataBaseInfo.Services;
using API.DTO.Requests;
using API.Extensions;
namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ImageService _imageService;
        public UserController(UserService userService, ImageService imageService)
        {
         
            _userService = userService;
       
            _imageService = imageService;
            }
        [HttpPost("UploadUserAvatar")]
        public async Task<IActionResult> UploadUserAvatar([FromForm] UploadAvatarRequest request)
        {
           
            Guid userId = User.GetUserIdAsGuidOrThrow();
            if (!ModelState.IsValid)
            {
                var errorMessages = string.Join(Environment.NewLine, ModelState.Values
         .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage));

                throw new AppException(new ErrorContext(
                    ServiceName.UserController,
                    OperationName.UploadUserAvatar,
                     HttpStatusCode.BadRequest,
                    "Вы указали некорректное изображение",
                    $"UserId: {userId}, Произошли ошибки валидации изображения: \n" + errorMessages));
            }

            var file = await _imageService.PrepareImageAsync(request.File, 512);
            string url = await _userService.UploadUserAvatarAsync(file, userId);

            return Ok(new
            {
                Url = url
            });
            
        }
    }
}