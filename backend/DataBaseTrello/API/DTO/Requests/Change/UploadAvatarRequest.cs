using System.ComponentModel.DataAnnotations;
using API.Attributes;
namespace API.DTO.Requests.Change
{
    public class UploadAvatarRequest
    {
        [Required(ErrorMessage = "Необходимо загрузить изображение")]
        [MaxFileSize(2*1024*1024)]
        [AllowedExtensions(".jpeg",".png",".jpg")]
        [AllowedMIMEType("image/jpeg","image/png","image/jpg")]
        [MinImageResoultion(512,512)]
        [MaxImageResoultion(2500, 2500)]
        public IFormFile File { get; set; }
    }
}
