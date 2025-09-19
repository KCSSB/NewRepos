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
        // Внешний ключ на Board (карточка принадлежит доске)
        public int BoardId { get; set; }
        public virtual Board? Board { get; set; } = null!;
        public virtual List<_Task> Tasks { get; set; } = new();


    }
}
