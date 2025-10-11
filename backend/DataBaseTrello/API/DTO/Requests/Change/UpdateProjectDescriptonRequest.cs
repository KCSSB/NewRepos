using System.ComponentModel.DataAnnotations;
using API.Constants;

namespace API.DTO.Requests.Change
{
    public record UpdateProjectDescriptonRequest
    {
        [Required(ErrorMessage = "Описание проекта обязательно!")]
        [MinLength(AllowLengthProp.MinProjectDescription)]
        [MaxLength(AllowLengthProp.MaxProjectDescription)]
        public string ProjectDescription { get; set; }
    }
}
