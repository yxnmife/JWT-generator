using CleanAuthenticator.Application.DTOs.Account;
using CleanAuthenticator.Application.Interfaces.Account;
using CleanAuthenticator.Application.Mappers;
using CleanAuthenticator.Domain;
using Microsoft.EntityFrameworkCore;

namespace CleanAuthenticator.Infrastructure
{
    public class AccountRepo : IAccountRepo
    {
        private readonly ApplicationDbContext _db;
      
        public AccountRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        

        public async Task<Accounts> CreateAccount(Accounts account)
        {
            var exists = _db.AccountsTable.Any(a => a.Username == account.Username || a.EmailAddress == account.EmailAddress);
            if (exists)
            {
                return null;
            }
            await _db.AccountsTable.AddAsync(account);
            await _db.SaveChangesAsync();
            return account;
        }

        public async Task DeleteAccountFromRepo(LoginDTO Login)
        {
            var user = await GetLoggedInAccount(Login);
            if (user != null)
            {
                _db.AccountsTable.Remove(user);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Account not found");
            }
        }

        public async Task<Accounts> GetAccountById(int id)
        {
            var account = await _db.AccountsTable.FindAsync(id);
            if(account == null)
            {
                throw new Exception($"Account with this ID does not exist");
            }
            return account;
        }

        public List<AccountDTO> GetAllAccounts()
        {
           return _db.AccountsTable.Select(a=>a.ToAccountDTO()).ToList();
        }

        public async Task<Accounts> GetbyEmail(string email)
        {
            var confirmedUser = await _db.AccountsTable.FirstOrDefaultAsync(x => x.EmailAddress.ToLower() == email.ToLower());
            if (confirmedUser == null)
            {
                throw new Exception("User Email not found");
            }
            return confirmedUser;
        }

        public async Task<Accounts> GetLoggedInAccount(LoginDTO login)
        {
            var user = await _db.AccountsTable.FirstOrDefaultAsync(x => x.Username.ToLower() == login.Username.ToLower());
            if (user == null)
            {
                throw new Exception("User does not exist");
            }
            return user;
        }
        public async Task SaveChangesToDB(Accounts accounts)
        {
            _db.AccountsTable.Update(accounts);
            _db.SaveChanges();
        }
    }
}
