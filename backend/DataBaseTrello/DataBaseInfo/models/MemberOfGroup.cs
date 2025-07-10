using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class MemberOfGroup
    {
        public int Id { get; set; }
        public string GroupRole { get; set; } = null!;
        public int ProjectUserId { get; set; }
        public virtual ProjectUser User { get; set; } = null!;

        public int GroupId { get; set; }
        public virtual Group Group { get; set; } = null!;
     
    }
}
