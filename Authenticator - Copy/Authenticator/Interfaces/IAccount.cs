using Authenticator.DTO.Account;
using Authenticator.Extensions;
using Authenticator.Models;

namespace Authenticator.Interfaces
{
    public interface IAccount
    {
        Task<Accounts> Create(CreateAccountDTO create);
        Task<HttpStatusResult> ChangePasswordValidation(Accounts? account);
        Task<Accounts> FindAccount(LoginDTO login);
        Task<Accounts> ChangePassword(Accounts person, ChangePasswordDTO change);
        Task<HttpStatusResult> AccountNonExistent(Accounts person);
        Task<Accounts> GetAccountFromCacheOrRepo(int id);
        Task<HttpStatusResult> LoginValidation(LoginDTO logindto);
        Task<HttpStatusResult> CreateAccountValidation(Accounts? data);
        Task<HttpStatusResult> DeleteAccountValidation(Accounts? user);
        Task<HttpStatusResult> ForgotPassword(string Email);
    }
}

