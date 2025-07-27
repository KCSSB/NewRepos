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
namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        private readonly TokenExtractorService _tokenExtractor;
        private readonly UserService _userService;
        public UserController(ILogger<ProjectsController> logger,TokenExtractorService tokenExtractor, UserService userService)
        {
            _logger = logger;
            _tokenExtractor = tokenExtractor;
            _userService = userService;
        }
        [HttpPost("UploadUserAvatar")]
        public async Task<IActionResult> UploadUserAvatar([FromForm] UploadAvatarRequest request)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken))
                throw new AppException(new ErrorContext(ServiceName.UserController,
                OperationName.UploadUserAvatar,
                HttpStatusCode.Unauthorized,
                UserExceptionMessages.UploadFilesExceptionMessage,
                "Не удалось получить данные из access token"));

            int userId = _tokenExtractor.TokenExtractorId(accessToken);

            string result = await _userService.UploadUserAvatarAsync(request.File, userId);
            
            return Ok();
        }
    }
}