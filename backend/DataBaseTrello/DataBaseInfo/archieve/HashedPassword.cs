using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseInfo.models
{
    public class HashedPassword
    {
        public string Password { get; set; } = null!;

        public byte[] storedHash { get; set; } = null!;
        public byte[] storedSalt { get; set; } = null!;

        public int userId { get; set; }
        public virtual User user { get; set; } = null!;
    }
}
