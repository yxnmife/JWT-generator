using System.ComponentModel.DataAnnotations;

namespace Authenticator.DTO.Account
{
    public class CreateAccountDTO
    {
        [Required]
        [MaxLength(15)]
        public string Username { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }
        [MinLength(8)]
        [MaxLength(14)]
        public string Password { get; set; }

    }
}
