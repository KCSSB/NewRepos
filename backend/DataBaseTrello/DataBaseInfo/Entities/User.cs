using System.ComponentModel.DataAnnotations;

namespace DataBaseInfo.models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; } = "Не указано";
        public string SecondName { get; set; } = "Не указано";
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public Guid InviteId { get; set; } = Guid.Empty;
        public Sex Sex { get; set; } = Sex.Unknown;
        public string Avatar {  get; set; } = string.Empty;
        public virtual List<ProjectUser> ProjectUsers { get; set; } = new();
        public virtual List<Session?> Sessions { get; set; } = new();
    }
    public enum Sex
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
    }
}
