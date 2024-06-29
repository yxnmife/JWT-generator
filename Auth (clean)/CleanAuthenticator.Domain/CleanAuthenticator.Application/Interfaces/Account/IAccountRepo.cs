using CleanAuthenticator.Application.DTOs.Account;
using CleanAuthenticator.Domain;

namespace CleanAuthenticator.Application.Interfaces.Account
{
    public interface IAccountRepo
    {
        List<AccountDTO> GetAllAccounts();
        Task<Accounts> CreateAccount(Accounts account);
        Task<Accounts> GetAccountById(int id);
        Task<Accounts> GetLoggedInAccount(LoginDTO login);
        Task DeleteAccountFromRepo(LoginDTO Login);
        Task SaveChangesToDB(Accounts accounts);
        Task<Accounts> GetbyEmail(string email);
    }
}
