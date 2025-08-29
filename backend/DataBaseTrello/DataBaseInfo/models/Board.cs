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
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

       public int LeadOfBoardId { get; set; }
        public List<MemberOfBoard> MemberOfBoards { get; set; } = new();
        public virtual List<Card> Cards { get; set; } = new();
    }
}
