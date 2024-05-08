using Authenticator.Data;
using Authenticator.DTO.Account;
using Authenticator.Interfaces;
using Authenticator.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authenticator.Repository.Account
{
    public class AccountRepo : IAccount

    {
        private readonly IPasswordLogic _passwordLogic;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        public AccountRepo(IPasswordLogic passwordLogic, ApplicationDbContext db, IConfiguration config)
        {
            _passwordLogic = passwordLogic;
            _db = db;
            _config = config;

        }
        public async Task<Accounts> Create(Accounts person, CreateAccountDTO create)
        {
           var exists= _db.AccountsTable.Any(a=>a.EmailAddress==create.EmailAddress);
            if (exists)
            {
                return null;
            }

            person.Username = create.Username;
            person.EmailAddress = create.EmailAddress;

            _passwordLogic.CreateHashPassword(create.Password, out byte[] passwordHash, out byte[] PasswordSalt);
            person.PasswordHash = passwordHash;
            person.PasswordSalt = PasswordSalt;
            return person;
        }

        public async Task<Accounts> ChangePassword(Accounts person, ChangePasswordDTO change)
        {
            if (change.New_Password != change.Confirm_New_Password)
            {
                return null;
            }
            else if (change.New_Password == change.Confirm_New_Password)
            {
              _passwordLogic.CreateHashPassword(change.New_Password, out byte[] passwordHash, out byte[] PasswordSalt);
                person.PasswordHash = passwordHash;
                person.PasswordSalt = PasswordSalt;
                return person;
               
            }
            return person;
        }
        public async Task<string> Login(LoginDTO login)
        {
            var user = await _db.AccountsTable.FirstOrDefaultAsync(x=>x.Username.ToLower()==login.Username.ToLower());
            if (user == null)
            {
                return null;
            }
            var VerifyPassword = _passwordLogic.VerifyPassword(login.Password,user.PasswordHash,user.PasswordSalt);
            if (!VerifyPassword)
            {
                return default;
            }
            return await CreateToken(user);
        }
        public async Task<Accounts> FindAccount(LoginDTO login)
        {
            var user = await _db.AccountsTable.FirstOrDefaultAsync(x => x.Username.ToLower() == login.Username.ToLower());
            if (user == null)
            {
                return null;
            }
            var isPasswordValid = _passwordLogic.VerifyPassword(login.Password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid)
            {
                return null;
            }
            return user;
        }
        public async Task<Accounts> FindAccountwithId(int Id)
        {
            var user = await _db.AccountsTable.FirstOrDefaultAsync(x =>x.Id==Id);
            if (user == null)
            {
                return null;
            }
            return user;
        }


        public async Task<Accounts> GetAccount(int id)
        {
            var person= _db.AccountsTable.FirstOrDefaultAsync(x => x.Id == id);
            if (person == null)
            {
                return null;
            }
            return await person;
        }
        public async Task<string> CreateToken(Accounts user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            List<Claim> ClaimsList = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName,user.Username),
                new Claim(ClaimTypes.Email,user.EmailAddress),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
              
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims: ClaimsList,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
