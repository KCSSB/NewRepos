using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class _Task
    {
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public DateOnly? DateOfDeadline { get; set; }
        public int MemberResponsibleForCard { get; set; } 
        public string Priority { get; set; } = string.Empty;
        public bool Complete { get; set; } = false;
        [MaxLength(1000)]
        public string? Description { get; set; } = string.Empty;
        // Внешний ключ на Card (задача принадлежит карточке)
        public int CardId { get; set; }
        public virtual Card? Card { get; set; }
    }
}
