using System.ComponentModel.DataAnnotations;

namespace Authenticator.Models
{
    public class Accounts
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public List<Portfolio> Portfolios { get; set; }=new List<Portfolio>();
    }
}
