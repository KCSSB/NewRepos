using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Внешний ключ на Board (карточка принадлежит доске)
        public int BoardId { get; set; }
        public virtual Board? Board { get; set; } = null!;
        public virtual List<_Task> Tasks { get; set; } = new();

        [NotMapped]
        public DateOnly? DateStartOfWork
        {
            get
            {
                if (Tasks == null || !Tasks.Any())
                    return null;

                var tasksWithStartDate = Tasks
                    .Where(t => t.DateOfStartWork != null)
                    .ToList();

                if (!tasksWithStartDate.Any())
                    return null;

                return tasksWithStartDate.Min(t => t.DateOfStartWork);
            }
        }

        [NotMapped]
        public DateOnly? DateOfDeadline
        {
            get
            {
                if (Tasks == null || !Tasks.Any())
                    return null;

                var tasksWithEndDate = Tasks
                    .Where(t => t.DateOfDeadline != null)
                    .ToList();

                if (!tasksWithEndDate.Any())
                    return null;

                return tasksWithEndDate.Max(t => t.DateOfDeadline);
            }
        }

    }
}
