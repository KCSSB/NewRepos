using System.ComponentModel.DataAnnotations;
using DataBaseInfo.models;

namespace DataBaseInfo.Entities
{
    public class SubTask
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public int TaskId { get; set; }
        public _Task Task { get; set; }
    }
}