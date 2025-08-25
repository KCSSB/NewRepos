using System.ComponentModel.DataAnnotations;
using API.Attributes;
namespace API.DTO.Requests
{
    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "Name of Project is Required!")]
        [MinLength(1)]
        [MaxLength(50)]
        public required string ProjectName { get; set; }

        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(".jpeg", ".png", ".jpg")]
        [AllowedMIMEType("image/jpeg", "image/png", "image/jpg")]
        [MinImageResoultion(1280, 720)]
        [MaxImageResoultion(4000,4000)]
        public IFormFile? image { get; set; }
    }
}
