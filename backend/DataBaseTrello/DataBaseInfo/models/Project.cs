using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ProjectName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public DateOnly DateStartWork { get; set; }
        public DateOnly? DateOfDeadline { get; set; }
        public virtual List<ProjectUser> ProjectUsers { get; set; } = new();

      
    }
}
