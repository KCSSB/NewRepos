using System.ComponentModel.DataAnnotations;

namespace API.DTO.Requests
{
    public record DeleteBoardsRequest
    {
        [Required(ErrorMessage = "Список должен быть не null!")]
        public List<int> BoardIds { get; set; } = new();
    }
}
