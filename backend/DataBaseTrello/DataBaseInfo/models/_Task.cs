using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class _Task
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public bool Complete { get; set; } = false;

        // Внешний ключ на Card (задача принадлежит карточке)
        public Guid CardId { get; set; }
        public virtual Card? Card { get; set; }
    }
}
