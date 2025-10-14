using System.ComponentModel.DataAnnotations;

namespace API.DTO.Requests.Delete
{
    public class DeleteProjectUsersRequest
    {
        [Required(ErrorMessage = "Список должен быть не Null!")]
        public List<int> ProjectUsers { get; set; } = new();
    }
}
