using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Configuration
{
    public class AuthSettings
    {
        public TimeSpan AccessTokenExpires {  get; set; }
        public TimeSpan RefreshTokenExpires { get; set; }
        public string SecretKey { get; set; } = null!;
    }
}
