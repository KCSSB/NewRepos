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
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = "Не указано";
        public string SecondName { get; set; } = "Не указано";
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public Guid InviteId { get; set; } = Guid.Empty;
        public Sex Sex { get; set; } = Sex.Unknown;
        public string Avatar {  get; set; } = string.Empty;
        public virtual List<ProjectUser> ProjectUsers { get; set; } = new();
        public virtual RefreshToken? RefreshToken { get; set; } 
    }
    public enum Sex
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
    }
}
