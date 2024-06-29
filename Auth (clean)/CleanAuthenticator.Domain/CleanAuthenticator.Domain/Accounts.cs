using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanAuthenticator.Domain
{
    public class Accounts
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // This attribute ensures auto-increment
        public int Id { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public List<Portfolio> Portfolios { get; set; } = new List<Portfolio>();

    }
}
