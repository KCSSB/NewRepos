using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class _Task
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool Complete { get; set; }

        // Внешний ключ на Card (задача принадлежит карточке)
        public int CardId { get; set; }
        public virtual Card Card { get; set; } = null!;
    }
}
