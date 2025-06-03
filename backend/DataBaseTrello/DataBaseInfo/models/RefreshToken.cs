using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataBaseInfo.models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
       

        public int UserId { get; set; }
        public virtual User user { get; set; } = null!;
    }
}
