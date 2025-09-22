using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class Board
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int LeadOfBoardId { get; set; }
        public List<MemberOfBoard> MemberOfBoards { get; set; } = new();
        public virtual List<Card> Cards { get; set; } = new();

        [NotMapped]
        public int ProgressBar
        {
            get
            {
                if (Cards == null || !Cards.Any())
                    return 0;

                var allSubTasks = Cards
                    .Where(c => c.Tasks != null)
                    .SelectMany(c => c.Tasks)
                    .Where(t => t.SubTasks != null)
                    .SelectMany(t => t.SubTasks)
                    .ToList();

                if (allSubTasks.Count == 0)
                    return 0;

                var completedSubTasks = allSubTasks.Count(st => st.IsCompleted);
                return (int)((double)completedSubTasks / allSubTasks.Count * 100);
            }
        }

        [NotMapped]
        public DateOnly? DateStartOfWork
        {
            get
            {
                // 1. Проверяем есть ли вообще карточки
                if (Cards == null || !Cards.Any())
                    return null;

                // 2. Собираем ВСЕ задачи из ВСЕХ карточек
                var allTasks = Cards
                    .Where(c => c.Tasks != null)          // Отфильтровали null карточки
                    .SelectMany(c => c.Tasks)             // Объединили все задачи
                    .Where(t => t.DateOfStartWork != null)      // Отфильтровали задачи без дат
                    .ToList();

                // 3. Если нет задач с датами - возвращаем null
                if (!allTasks.Any())
                    return null;

                // 4. Возвращаем минимальную дату (DateOnly)
                return allTasks.Min(t => t.DateOfStartWork);
            }
        }
        [NotMapped]
        public DateOnly? DateOfDeadline
        {
            get
            {
                // 1. Проверяем есть ли вообще карточки
                if (Cards == null || !Cards.Any())
                    return null;

                // 2. Собираем ВСЕ задачи из ВСЕХ карточек
                var allTasks = Cards
                    .Where(c => c.Tasks != null)          // Отфильтровали null карточки
                    .SelectMany(c => c.Tasks)             // Объединили все задачи
                    .Where(t => t.DateOfDeadline != null)  // Отфильтровали задачи без дат окончания
                    .ToList();

                // 3. Если нет задач с датами окончания - возвращаем null
                if (!allTasks.Any())
                    return null;

                // 4. Возвращаем МАКСИМАЛЬНУЮ дату (последний дедлайн)
                return allTasks.Max(t => t.DateOfDeadline);
            }
        }
    }
}
