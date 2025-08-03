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
        public string GroupRole { get; set; } = string.Empty;
        public int ProjectUserId { get; set; }
        public virtual ProjectUser? User { get; set; }
        public int GroupId { get; set; }
        public virtual Group? Group { get; set; }
     
    }
}
