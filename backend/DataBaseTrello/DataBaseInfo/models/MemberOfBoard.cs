using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class MemberOfBoard
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string BoardRole { get; set; } = string.Empty;
        public Guid ProjectUserId { get; set; }
        public virtual ProjectUser? ProjectUser { get; set; }
        public Guid BoardId { get; set; }
        public virtual Board? Board { get; set; }
     
    }
}
