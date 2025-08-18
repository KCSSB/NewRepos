using System.ComponentModel.DataAnnotations;
using API.Attributes;
namespace API.DTO.Requests
{
    public class UploadAvatarRequest
    {
        [Required(ErrorMessage = "Необходимо загрузить изображение")]
        [MaxFileSize(2*1024*1024)]
        [AllowedExtensions(".jpeg",".png",".jpg")]
        [AllowedMIMEType("image/jpeg","image/png","image/jpg")]
        [MinImageResoultion(512,512)]
        public IFormFile File { get; set; }
    }
}
