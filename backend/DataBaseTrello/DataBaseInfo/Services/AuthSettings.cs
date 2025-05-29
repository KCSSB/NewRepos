using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseInfo.Services
{
    public class AuthSettings
    {
        public TimeSpan expires {  get; set; }
        public string SecretKey { get; set; }
    }
}
