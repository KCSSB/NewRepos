using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; } = null!;

        public virtual List<MemberOfGroup> Members { get; set; } = new(); // ок

        public int LeadId { get; set; }
        public virtual ProjectUser Lead { get; set; } = null!;

        public virtual List<Board> Boards { get; set; } = new();
    }
}
