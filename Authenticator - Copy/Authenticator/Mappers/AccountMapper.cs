using Authenticator.DTO.Account;
using Authenticator.Models;
using System.Net.Mail;
using System.Runtime.CompilerServices;

namespace Authenticator.Mappers
{
    public static class AccountMapper
    {
        public static AccountDTO ToAccountDTO(this Accounts accounts)
        {
            return new AccountDTO
            {
                Id = accounts.Id,
                Username= accounts.Username,
                EmailAddress= accounts.EmailAddress,
            };
        }
    }
}
