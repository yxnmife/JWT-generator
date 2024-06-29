using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.DTOs.Account
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
