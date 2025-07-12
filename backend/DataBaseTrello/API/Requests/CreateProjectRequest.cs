using System.ComponentModel.DataAnnotations;

namespace API.Requests
{
    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "Name of Project is Required!")]
        [MinLength(1)]
        [MaxLength(50)]
        public required string ProjectName { get; set; }
    }
}
