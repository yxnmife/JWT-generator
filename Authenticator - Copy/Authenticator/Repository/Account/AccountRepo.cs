using Authenticator.Caching;
using Authenticator.Data;
using Authenticator.DTO.Account;
using Authenticator.Extensions;
using Authenticator.Interfaces;
using Authenticator.Mappers;
using Authenticator.Models;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;


namespace Authenticator.Repository.Account
{
    public class AccountRepo : IAccount

    {
        private readonly IPasswordLogic _passwordLogic;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly ICacheProvider _cache;
        public AccountRepo(IPasswordLogic passwordLogic, ApplicationDbContext db, IConfiguration config,ICacheProvider cache)
        {
            _passwordLogic = passwordLogic;
            _db = db;
            _config = config;
            _cache = cache;
        }
        public async Task<Accounts> Create(CreateAccountDTO create)
        {
            var person = new Accounts();
           var exists= _db.AccountsTable.Any(a=> a.Username==create.Username && a.EmailAddress == create.EmailAddress);
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
        private async Task<string> Login(LoginDTO login)
        {
            var user = await _db.AccountsTable.FirstOrDefaultAsync(x=>x.Username.ToLower()==login.Username.ToLower());
            if (user == null)
            {
                throw new Exception("User does not exist");
            }
            var VerifyPassword = _passwordLogic.VerifyPassword(login.Password,user.PasswordHash,user.PasswordSalt);
            if (!VerifyPassword)
            {
                throw new Exception("Incorrect password");
            }
            return await CreateToken(user);
        }
      public async Task<HttpStatusResult> LoginValidation(LoginDTO logindto)
        {
            try
            {
                var token= await Login(logindto);
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = new List<string> {
                    "Message: Success",
                    $"Token: {token} "
                }
                };
            }
            catch(Exception ex)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = new List<string> 
                    {
                        "Response: Unauthorized",
                        $"ErrorCode: {(int)HttpStatusCode.Unauthorized}",
                        $"Message: {ex.Message}"
                    },
                };
            }
        }
        private async Task<Accounts?> GetAccount(int id)
        {
            var person = _db.AccountsTable.FirstOrDefaultAsync(x => x.Id == id);
            if (person == null)
            {
                return null;
            }
            return await person;
        }
        public async Task<Accounts?> GetAccountFromCacheOrRepo(int id)
        {
            if (!_cache.TryGetValue(CacheKeys.GetAccount + id, out Accounts person))
            {
                person = await GetAccount(id);
                if(person == null)
                {
                    return person = null;
                }
                else
                {
                    var CacheEntryOption = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(1),
                        SlidingExpiration = TimeSpan.FromSeconds(15)
                    };
                    _cache.Set(CacheKeys.GetAccount + id, person, CacheEntryOption);
                }
            }

            return person;
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
       
        public async Task<HttpStatusResult> AccountNonExistent(Accounts person)
        {
            if (person == null)
            {
                return new HttpStatusResult()
                {
                    StatusCode=HttpStatusCode.NotFound,
                    Message=new List<string>
                    {
                        "Response: Bad Request",
                        "ErrorCode: 404",
                        "Message: Account Not Found" 
                    },
                };
            }
            else
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message= person.ToAccountDTO(),
                  
                };
            };
        }

        public async Task<HttpStatusResult> CreateAccountValidation(Accounts? data )
        {
            try
            {
                if (data == null)
                {
                    return new HttpStatusResult()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = new List<string>()
                    {
                        "Response: Bad Request",
                        $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                        $"Message: User/Email address already exists"
                    }
                    };
                }

                await _db.AccountsTable.AddAsync(data);
                await _db.SaveChangesAsync();
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = data.ToAccountDTO()
                };
            }
            catch(DuplicateNameException ex)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = new List<string>()
                    {
                        "Response: Bad Request",
                        $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                        $"Message: {ex.Message}"
                    }
                };
            }
            catch(Exception ex)
            {

                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = new List<string>()
                    {
                        "Response: Bad Request",
                        $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                        $"Message: Unknown Error"
                    }
                };
            }
        }


    public async Task<HttpStatusResult> ChangePasswordValidation(Accounts? account)
        {
            if (account == null)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = new List<string>
                    {
                        "Response: Bad Request",
                        "ErrorCode: 404",
                        "Message: Passwords do not match" 
                    }
                };
            }
            await _db.SaveChangesAsync();
            return new HttpStatusResult()
            {
                StatusCode = HttpStatusCode.OK,
                Message = account.ToAccountDTO()
            };
        }

        public async Task<HttpStatusResult> DeleteAccountValidation(Accounts? user)
        {
            if (user == null)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = new List<string>
                    {
                        "Response: Bad Request",
                        "ErrorCode: 404",
                        "Message: User Not Found / Incorrect Password"
                    }
                };
                
            }
            _db.AccountsTable.Remove(user);
            await _db.SaveChangesAsync();

            return new HttpStatusResult()
            {
                StatusCode = HttpStatusCode.OK,
                Message = user.ToAccountDTO()
            };
        }
       
        private async Task<string> CreateToken(Accounts user)
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

        private string RandomPassword()
        {
            Random random = new Random();

            const int length = 10;
            const string digits = "1234567890";
            const string lowercase = "qwertyuiopasdfghjklzxcvbnm";
            const string uppercase = "QWERTYUIOPASDFGHJKLZXCVBNM";
            const string specialchar = "!@#$%^&*()_+:{};.><";

            var Allcharacters = digits + lowercase + uppercase + specialchar;

            var Password = new List<char>(length);
            for (var i = 0; i < length; i++)
            {
                Password.Add(Allcharacters[random.Next(Allcharacters.Length)]);

            }
            string? PasswordString = new string(Password.ToArray());
            return PasswordString;

        }

        public async Task<HttpStatusResult> ForgotPassword(string Email)
        {
            try
            {
                var confirmedUser = await _db.AccountsTable.FirstOrDefaultAsync(a => a.EmailAddress == Email);
                if (confirmedUser == null)
                {
                    throw new Exception("User Email not found");
                }

                var randomPassword = RandomPassword();
                _passwordLogic.CreateHashPassword(randomPassword, out byte[] passwordHash, out byte[] PasswordSalt);

                confirmedUser.PasswordHash = passwordHash;
                confirmedUser.PasswordSalt = PasswordSalt;

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("curtisdave86@gmail.com"));
                email.To.Add(MailboxAddress.Parse($" {confirmedUser.EmailAddress}"));

                email.Subject = "Your new Login Password!! ";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = $"Your new login password is <b>{randomPassword}</b>" };

                using var smtp = new SmtpClient();

                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate("curtisdave86@gmail.com", "kejq gqjg ymfd ziwl");
                smtp.Send(email);
                smtp.Disconnect(true);

                await _db.SaveChangesAsync();

                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = new List<string>()
                {
                    "Response: success",

                    $"Message: Your new password has been sent to your email Address."
                }
                };

            }

            catch (Exception ex)
            {

                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = new List<string>()
                    {
                    "Response: Bad Request",
                    "ErrorCode: 400",
                    $"Message: {ex.Message}"
                    }
                };
            }
           
        }
    }
}
