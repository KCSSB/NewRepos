using System.ComponentModel.DataAnnotations;
using API.Constants;

namespace API.DTO.Requests.Create
{
    public class CreateBoardRequest
    {
        [Required(ErrorMessage = "Вы не указали название доски")]
        [MinLength(AllowLengthProp.MinBoardName)]
        [MaxLength(AllowLengthProp.MaxBoardName)]
        public required string BoardName { get; set; }
        }
}
