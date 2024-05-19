using System.ComponentModel.DataAnnotations;

namespace Authenticator.DTO.Account
{
    public class ChangePasswordDTO
    {
        [MinLength(8)]
        [MaxLength(14)]
        public string New_Password { get; set; }
        [MinLength(8)]
        [MaxLength(14)]
        public string Confirm_New_Password { get; set; }

    }
}
