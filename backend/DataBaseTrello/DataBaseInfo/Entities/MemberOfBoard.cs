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
        public int Id { get; set; }
        public string BoardRole { get; set; } = string.Empty;
        public int ProjectUserId { get; set; }
        public virtual ProjectUser? ProjectUser { get; set; }
        public int BoardId { get; set; }
        public virtual Board? Board { get; set; }
     
    }
}
