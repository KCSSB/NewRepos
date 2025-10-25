using System.ComponentModel.DataAnnotations;

namespace API.DTO.Requests.Change
{
    public record UpdateBoardsNameRequest
    {
        [Required(ErrorMessage = "Список должен быть не Null!")]
        public List<UpdatedBoard> UpdatedBoards { get; set; } = new();
    }
}
