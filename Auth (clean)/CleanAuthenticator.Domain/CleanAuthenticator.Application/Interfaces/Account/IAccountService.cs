using CleanAuthenticator.Application.DTOs.Account;

namespace CleanAuthenticator.Application.Interfaces.Account
{
    public interface IAccountService
    {
        HttpStatusResult GetAllAccounts();
        Task<HttpStatusResult> CreateAccount(CreateAccountDTO create);
        Task<HttpStatusResult> GetAccountById(int id);
        Task<HttpStatusResult> Login(LoginDTO login);
        Task<HttpStatusResult> DeleteAccount(LoginDTO login);
        Task<HttpStatusResult> ChangePassword(string Id, ChangePasswordDTO change);
        Task<HttpStatusResult> ForgotPassword(string email);
    }
}
