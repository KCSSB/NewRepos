using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class Board
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        // Внешний ключ на Group (Доска принадлежит группе)
        public Guid GroupId { get; set; }
        public virtual Group? Group { get; set; } = null!;

        public virtual List<Card> Cards { get; set; } = new();
    }
}
