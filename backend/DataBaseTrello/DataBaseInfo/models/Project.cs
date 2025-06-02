using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class Project
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = null!;

        public virtual List<ProjectUser> ProjectUsers { get; set; } = new();
        public virtual List<Group> Groups { get; set; } = new();
        public virtual List<Board> Boards { get; set; } = new();

    }
}
