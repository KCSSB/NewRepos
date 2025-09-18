using System.ComponentModel.DataAnnotations;
using DataBaseInfo.Entities;


namespace DataBaseInfo.models
{
    public class _Task
    {
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public DateOnly? DateOfStartWork { get; set; }
        public DateOnly? DateOfDeadline { get; set; }
        public int TaskResponsibleMembersц { get; set; } 
        public string Priority { get; set; } = string.Empty;
        public bool Complete { get; set; } = false;
        [MaxLength(1000)]
        public string? Description { get; set; } = string.Empty;
        public List<SubTask> SubTasks { get; set; }
        // Внешний ключ на Card (задача принадлежит карточке)
        public int CardId { get; set; }
        public virtual Card? Card { get; set; }
    }
}
