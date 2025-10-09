using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DataBaseInfo.Entities;


namespace DataBaseInfo.models
{
    public class _Task
    {
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; } = "Новая задача";
        public DateOnly? DateOfStartWork { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly? DateOfDeadline { get; set; } = DateOnly.FromDateTime(DateTime.Now).AddDays(60);
        public string Priority { get; set; } = "Высокий";
        [MaxLength(1000)]
        public string? Description { get; set; } = "Описание задачи";
        public virtual List<SubTask> SubTasks { get; set; } = new();
        // Внешний ключ на Card (задача принадлежит карточке)
        public int CardId { get; set; }
        public virtual Card? Card { get; set; }

        public virtual List<ResponsibleForTask> Responsibles { get; set; } = new(); 

        [NotMapped]
        public int Progress
        {
            get
            {
                if (SubTasks == null || SubTasks.Count == 0)
                    return 0;

                var completedCount = SubTasks.Count(st => st.IsCompleted);
                return completedCount / SubTasks.Count;
            }
        }
        [NotMapped]
        public List<int> ResponsibleIds => Responsibles?
        .Select(r => r.MemberOfBoardId)
        .ToList() ?? new List<int>();

    }
}
