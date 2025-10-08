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
        public virtual List<MemberOfBoard> MemberOfBoards { get; set; } = new();
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
     
                if (Cards == null || !Cards.Any())
                    return null;

        
                var allTasks = Cards
                    .Where(c => c.Tasks != null)    
                    .SelectMany(c => c.Tasks)            
                    .Where(t => t.DateOfStartWork != null)     
                    .ToList();

          
                if (!allTasks.Any())
                    return null;

            
                return allTasks.Min(t => t.DateOfStartWork);
            }
        }
        [NotMapped]
        public DateOnly? DateOfDeadline
        {
            get
            {
          
                if (Cards == null || !Cards.Any())
                    return null;

            
                var allTasks = Cards
                    .Where(c => c.Tasks != null)    
                    .SelectMany(c => c.Tasks)            
                    .Where(t => t.DateOfDeadline != null)  
                    .ToList();

           
                if (!allTasks.Any())
                    return null;

            
                return allTasks.Max(t => t.DateOfDeadline);
            }
        }
    }
}
