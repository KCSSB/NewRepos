using System.ComponentModel.DataAnnotations;
using API.Attributes;
namespace API.DTO.Requests
{
    public class UploadAvatarRequest
    {
        [Required(ErrorMessage = "Необходимо загрузить изображение")]
        [MaxFileSize(2*1024*1024)]
        [AllowedExtensions(new[] {".jpeg",".png","jpg"})]
        public IFormFile File { get; set; }
    }
}
