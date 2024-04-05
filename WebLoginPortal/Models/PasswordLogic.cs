using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace WebLoginPortal.Models
{
    public class PasswordLogic : IPasswordLogic
    { 
        public void CreateHashPassword(string password,out byte[] passwordHash,out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())

            {
                passwordSalt = hmac.Key;

                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                
            }
        }
        public bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedhash.SequenceEqual(passwordHash);
            }
        }

        public byte[] HashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())

            {
                passwordSalt = hmac.Key;

                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            }
            return passwordHash;
        }
       

    }
}
