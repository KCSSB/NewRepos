using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public Guid InviteId { get; set; } = Guid.Empty;
        public string Sex { get; set; } = string.Empty;
        public string Avatar {  get; set; } = string.Empty;
        public virtual List<ProjectUser> ProjectUsers { get; set; } = new();
        public virtual RefreshToken? RefreshToken { get; set; } 
    }
}
