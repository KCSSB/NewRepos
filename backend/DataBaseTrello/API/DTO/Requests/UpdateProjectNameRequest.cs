using System.ComponentModel.DataAnnotations;
using API.Constants;

namespace API.DTO.Requests
{
    public class UpdateProjectNameRequest
    {
        [Required(ErrorMessage = "Название проекта обязательно!")]
        [MinLength(AllowLengthProp.MinProjectName)]
        [MaxLength(AllowLengthProp.MaxProjectName)]
        public string UpdatedProjectName { get; set; }
    }
}
