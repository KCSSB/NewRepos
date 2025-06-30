using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;


namespace DataBaseInfo.Services
{
    public class HashService
    {
        private readonly byte[] _secretKey;

        public HashService(IOptions<AuthSettings> options)
        {
            _secretKey = Encoding.UTF8.GetBytes(options.Value.SecretKey);
        }

        public string HashToken(string token)
        {
            using var hmac = new HMACSHA256(_secretKey);
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyToken(string token, string hashedToken)
        {
            return HashToken(token) == hashedToken;
        }
    }
}
