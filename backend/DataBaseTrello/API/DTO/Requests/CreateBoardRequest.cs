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
        public required Guid BoardLeadId { get; set; }
        public required Guid ProjectId { get; set; }
        public List<Guid> BoardMembers { get; set; } = new List<Guid>();
        }
}
