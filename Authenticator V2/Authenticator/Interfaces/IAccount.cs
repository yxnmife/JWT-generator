using Authenticator.DTO.Account;
using Authenticator.Extensions;
using Authenticator.Models;

namespace Authenticator.Interfaces
{
    public interface IAccount
    {
        Task<Accounts> Create(CreateAccountDTO create);
        Task<HttpStatusResult> ChangePasswordValidation(Accounts? account);
        Task<string> Login(LoginDTO login);
        Task<Accounts> FindAccount(LoginDTO login);
        Task<Accounts> ChangePassword(Accounts person, ChangePasswordDTO change);
        Task<HttpStatusResult> AccountNonExistent(Accounts person);
        Task<Accounts> GetAccountFromCacheOrRepo(int id);
        Task<HttpStatusResult> LoginValidation(string token);
        Task<HttpStatusResult> CreateAccountValidation(Accounts? data);
        Task<HttpStatusResult> DeleteAccountValidation(Accounts? user);

    }
}

