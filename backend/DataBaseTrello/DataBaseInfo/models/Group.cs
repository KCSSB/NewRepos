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
        public string Name { get; set; } = string.Empty;
        public int LeadId { get; set; }
        public virtual List<MemberOfGroup> Members { get; set; } = new(); // ок
        public virtual List<Board> Boards { get; set; } = new();
    }
}
