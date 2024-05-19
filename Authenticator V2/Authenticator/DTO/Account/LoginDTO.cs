using System.ComponentModel.DataAnnotations;

namespace Authenticator.DTO.Account
{
    public class LoginDTO
    {
        [MaxLength(15)]
        public string Username { get; set; }
        [MinLength(8)]
        [MaxLength(14)]
        public string Password { get; set; }
    }
}
