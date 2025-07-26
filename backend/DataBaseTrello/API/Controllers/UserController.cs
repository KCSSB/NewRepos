using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Imagekit;
using Imagekit.Sdk;
using Microsoft.Extensions.Options;
using API.Configuration;
namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        private readonly ImagekitClient _imagekitClient;
        private readonly ImageKitSettings _settings;
        public UserController(ILogger<ProjectsController> logger,IOptions<ImageKitSettings> options)
        {
            _logger = logger;
            _settings = options.Value;
            _imagekitClient = new ImagekitClient(publicKey: _settings.PublicKey,
                privateKey: _settings.PrivateKey,
                urlEndPoint: _settings.UrlEndpoint);
            
           
        }
        [HttpPost("UploadUserAvatar")]
        public async Task<IActionResult> UploadUserAvatar([FromForm] IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var request = new FileCreateRequest
            {
                file = stream,
                fileName = file.FileName,
                useUniqueFileName = true
            };
            var result = await _imagekitClient.UploadAsync(request);
            //ПРоверка успеха операции и добавоение в базу данных url, 
            return Ok(new
            {
                url = result.url,
            });
        }
    }
}