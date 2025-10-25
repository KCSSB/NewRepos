using System.ComponentModel.DataAnnotations;
using API.Constants;

namespace API.DTO.Requests.Change
{
    public record UpdatedBoard
    {
        [Required(ErrorMessage = "Вы не указали Id доски")]
        public int BoardId { get; set; }
        [Required(ErrorMessage = "Вы не указали новое имя доски")]
        [MinLength(AllowLengthProp.MinBoardName)]
        [MaxLength(AllowLengthProp.MaxBoardName)]
        public string UpdatedName { get; set; }
    }
}
