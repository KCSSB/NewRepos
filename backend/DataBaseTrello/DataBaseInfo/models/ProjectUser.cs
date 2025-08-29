using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class ProjectUser
    {
        [Key]
        public int Id { get; set; }
        public string ProjectRole { get; set; } = string.Empty;//owner или member
        public int UserId { get; set; }
        public virtual User? User { get; set; } 
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; } = null!;
        public virtual List<MemberOfBoard> MembersOfBoards { get; set; } = new();

    }
}
