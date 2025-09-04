using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Constants;
using API.Exceptions.Context;
using System.Net;
using API.DTO.Requests;
using API.Extensions;
using API.DTO.Mappers;
using API.Middleware;
using API.Services.Helpers;
using API.Services.Application.Implementations;
using API.Exceptions.ContextCreator;
namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ImageService _imageService;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;


        public UserController(UserService userService, ImageService imageService, IErrorContextCreatorFactory errCreatorFactory)
            {
         
        _errCreatorFactory = errCreatorFactory;
            _userService = userService;
            _imageService = imageService;
        
            }
private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(UserController));
        [HttpPatch("UpdateGeneralUserInfo")]
        public async Task<IActionResult> UpdateGeneralUserInfo([FromBody]UpdateUserRequest request)
        {
            var userId = User.GetUserId();
            var userInfoModel = ToDomainMapper.ToUpdateUserModel(request);
            var updatedUser = await _userService.UpdateUserAsync(userInfoModel, userId);
            var response = ToResponseMapper.ToUpdateUserResponse(updatedUser);
            return Ok(response);
        } 
        [HttpPost("UploadUserAvatar")]
        public async Task<IActionResult> UploadUserAvatar([FromForm] UploadAvatarRequest request)
        {
            int userId = User.GetUserId();
            if (!ModelState.IsValid)
            {
                var errorMessages = string.Join(Environment.NewLine, ModelState.Values
         .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage));

                throw new AppException(_errCreator.BadRequest($"UserId: {userId}, Произошли ошибки валидации изображения: \n" + errorMessages));
            }

      
            var result = await _imageService.UploadImageAsync(request.File, CloudPathes.UserAvatarPath);
            var url = await _userService.UpdateUserAvatarAsync(result, userId);
            return Ok(new
            {
                Url = url
            });
        
        }
        [HttpPatch("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.GetUserId();
            await _userService.ChangePasswordAsync(request.OldPassword, request.NewPassword, userId);
            return Ok("Пароль успешно изменён");
        }
    }
}