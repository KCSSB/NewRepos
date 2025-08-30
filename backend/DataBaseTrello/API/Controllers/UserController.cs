using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Constants;
using API.Exceptions.Context;
using System.Net;
using API.Helpers;
using DataBaseInfo.Services;
using API.DTO.Requests;
using API.Extensions;
using API.DTO.Mappers.ToDomainModel;
using API.DTO.Mappers.ToResponseModel;
using API.Exceptions;
namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ImageService _imageService;
        private readonly ErrorContextCreator _errCreator;
        public UserController(UserService userService, ImageService imageService)
            {
         
            _userService = userService;
            _imageService = imageService;
            _errCreator = new ErrorContextCreator(ServiceName.UserController);
            }
        [HttpPatch("UpdateGeneralUserInfo")]
        public async Task<IActionResult> UpdateGeneralUserInfo([FromBody]UpdateUserRequest request)
        {
            var userId = User.GetUserId();
            var userInfoModel = ToDomainModelMapper.ToUpdateUserModel(request);
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