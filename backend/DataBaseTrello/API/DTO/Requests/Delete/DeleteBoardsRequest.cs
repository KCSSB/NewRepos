using System.ComponentModel.DataAnnotations;

namespace API.DTO.Requests.Delete
{
    public record DeleteBoardsRequest
    {
        [Required(ErrorMessage = "Список должен быть не null!")]
        public List<int> BoardIds { get; set; } = new();
    }
}
