using System.Security.Cryptography;
using System.Text;

namespace API.Helpers
{
    public class PasswordHasher //Не используется
    {
        public void CreatePasswordHash(string password, out byte[] hash, out byte[]salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using(var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }
    }
}
