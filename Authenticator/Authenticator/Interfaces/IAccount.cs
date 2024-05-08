using Authenticator.DTO.Account;
using Authenticator.Models;

namespace Authenticator.Interfaces
{
    public interface IAccount
    {
      Task<Accounts> Create(Accounts person, CreateAccountDTO create);
      Task<Accounts> GetAccount(int id);
      Task<string> Login(LoginDTO login);
      Task<Accounts> FindAccount(LoginDTO login);
      Task<Accounts> FindAccountwithId(int Id);
      Task<Accounts> ChangePassword(Accounts person, ChangePasswordDTO change);


    }
}

