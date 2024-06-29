using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.DTOs.Account
{
    public class AccountDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
    }
}
