using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.DTOs.Account
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
