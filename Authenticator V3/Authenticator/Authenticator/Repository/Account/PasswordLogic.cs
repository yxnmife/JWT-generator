using Authenticator.Data;
using Authenticator.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Authenticator.Repository.Account
{
    public class PasswordLogic : IPasswordLogic
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public PasswordLogic(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _config = configuration;
        }

        public void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
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
    }
}
