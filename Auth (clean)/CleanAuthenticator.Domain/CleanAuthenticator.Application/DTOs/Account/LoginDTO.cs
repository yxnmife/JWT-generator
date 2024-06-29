using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.DTOs.Account
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
