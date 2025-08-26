using System.ComponentModel.DataAnnotations;
using API.Attributes;
using API.Constants;
namespace API.DTO.Requests
{
    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "Вы не указали название проекта")]
        [MinLength(AllowLengthProp.MinProjectName)]
        [MaxLength(AllowLengthProp.MaxProjectName)]
        public required string ProjectName { get; set; }

        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(".jpeg", ".png", ".jpg")]
        [AllowedMIMEType("image/jpeg", "image/png", "image/jpg")]
        [MinImageResoultion(1280, 720)]
        [MaxImageResoultion(4000,4000)]
        public IFormFile? image { get; set; }
    }
}
