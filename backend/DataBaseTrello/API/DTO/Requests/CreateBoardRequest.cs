using System.ComponentModel.DataAnnotations;
using API.Constants;

namespace API.DTO.Requests
{
    public class CreateBoardRequest
    {
        [Required(ErrorMessage = "Вы не указали название доски")]
        [MinLength(AllowLengthProp.MinBoardName)]
        [MaxLength(AllowLengthProp.MaxBoardName)]
        public required string BoardName { get; set; }
        public required int BoardLeadId { get; set; }
        public required int ProjectId { get; set; }
        public List<int> BoardMembers { get; set; } = new List<int>();
        }
}
