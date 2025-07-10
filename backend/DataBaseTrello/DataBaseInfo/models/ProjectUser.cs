using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class ProjectUser
    {
        public int Id { get; set; }
        public string projectRole { get; set; } = null!;//owner или member
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; } = null!;

        public virtual List<MemberOfGroup> Groups { get; set; } = new();

    }
}
