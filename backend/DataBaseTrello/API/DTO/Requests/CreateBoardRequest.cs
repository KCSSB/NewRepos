using System.ComponentModel.DataAnnotations;

namespace API.DTO.Requests
{
    public class CreateBoardRequest
    {
        [Required(ErrorMessage = "Board's Name is required!")]
        [MinLength(1)]
        [MaxLength(50)]
        public required string BoardName { get; set; }
        public required Guid BoardLeadId { get; set; }
        public required Guid ProjectId { get; set; }
        public List<Guid> BoardMembers { get; set; } = new List<Guid>();
        }
}
