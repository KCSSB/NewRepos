using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Range(0,100)]
        public int Progress { get; set; } = 0;

        // Внешний ключ на Board (карточка принадлежит доске)
        public int BoardId { get; set; }
        public virtual Board? Board { get; set; } = null!;
        public virtual List<_Task> Tasks { get; set; } = new();


    }
}
