using System.ComponentModel.DataAnnotations;
using DataBaseInfo.models;

namespace DataBaseInfo.Entities
{
    public class SubTask
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "Новая подзадача";
        public bool IsCompleted { get; set; } = false;
        public int TaskId { get; set; }
        public virtual _Task Task { get; set; }
    }
}