using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class Board
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Внешний ключ на Group (Доска принадлежит группе)
        public int GroupId { get; set; }
        public virtual Group Group { get; set; } = null!;

        public virtual List<Card> Cards { get; set; } = new();
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; } = null!;
    }
}
