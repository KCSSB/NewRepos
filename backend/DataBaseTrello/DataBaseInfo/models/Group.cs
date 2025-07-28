using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class Group
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public Guid LeadId { get; set; }
        public virtual List<MemberOfGroup> Members { get; set; } = new(); // ок
        public virtual List<Board> Boards { get; set; } = new();
    }
}
