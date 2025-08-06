using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class MemberOfGroup
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string GroupRole { get; set; } = string.Empty;
        public Guid ProjectUserId { get; set; }
        public virtual ProjectUser? User { get; set; }
        public Guid GroupId { get; set; }
        public virtual Group? Group { get; set; }
     
    }
}
