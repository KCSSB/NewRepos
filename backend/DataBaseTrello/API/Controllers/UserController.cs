using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Constants;
using API.Exceptions.ErrorContext;
using System.Net;
using API.Helpers;
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
        [HttpPatch("UpdateGeneralInfo")]
        public async Task<IActionResult> UpdateGeneralInfo()
        {

        } 
        [HttpPost("UploadUserAvatar")]
        public async Task<IActionResult> UploadUserAvatar([FromForm] UploadAvatarRequest request)
        {
            Guid userId = User.GetUserId();
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

      
            var result = await _imageService.UploadImageAsync(request.File, CloudPathes.UserAvatarPath);
            var url = await _userService.UpdateUserAvatarAsync(result, userId);
            return Ok(new
            {
                Url = url
            });
        
        }
    }
}