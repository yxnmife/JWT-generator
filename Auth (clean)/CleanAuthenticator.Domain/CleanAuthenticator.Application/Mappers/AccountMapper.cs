using CleanAuthenticator.Application.DTOs.Account;
using CleanAuthenticator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.Mappers
{
    public static class AccountMapper
    {
        public static AccountDTO ToAccountDTO(this Accounts accounts)
        {
            return new AccountDTO
            {
                Id = accounts.Id,
                Username = accounts.Username,
                EmailAddress = accounts.EmailAddress,
            };
        }
    }
}
