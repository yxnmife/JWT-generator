using CleanAuthenticator.Application.DTOs.Account;
using CleanAuthenticator.Application.Mappers;
using CleanAuthenticator.Domain;

using MimeKit;
using MailKit.Net.Smtp;
using System.Net;
using CleanAuthenticator.Application.Interfaces.Account;


namespace CleanAuthenticator.Application.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepo;
        private readonly IPasswordLogic _passwordLogic;
        private readonly IJwtgenerator _jwtGenerator;
        public AccountService(IAccountRepo accountRepo, IPasswordLogic passwordLogic, IJwtgenerator jwtgenerator)
        {
            _accountRepo = accountRepo;
            _passwordLogic = passwordLogic;
            _jwtGenerator = jwtgenerator;
        }


        //below is what will talk to the presentation layer
        public async Task<HttpStatusResult> ChangePassword(string Id, ChangePasswordDTO change)
        {
            try
            {
                var ID = Convert.ToInt32(Id);
                var person = await _accountRepo.GetAccountById(ID);

                if (person == null)
                {
                    return new HttpStatusResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = new List<string>
                    {
                        "Response: Not Found",
                        $"ErrorCode: {(int)HttpStatusCode.NotFound}",
                        "Message: Account Not Found"
                    },
                    };
                }

                if (change.New_Password != change.Confirm_New_Password)
                {
                    return new HttpStatusResult()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = new List<string>
                    {
                        "Response: Bad Request",
                        $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                        "Message: Passwords do not match"
                    }
                    };
                }

                _passwordLogic.CreateHashPassword(change.New_Password, out byte[] passwordHash, out byte[] PasswordSalt);
                person.PasswordHash = passwordHash;
                person.PasswordSalt = PasswordSalt;

                await _accountRepo.SaveChangesToDB(person);

                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = new List<string>
                {
                    $"UserId: {person.Id}",
                        $"Email Address: {person.EmailAddress}",
                        "Message: Password changed successfully"
                    }
                };

            }
            catch (Exception ex)
            {
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
            }
        }

        public async Task<HttpStatusResult> CreateAccount(CreateAccountDTO create)
        {
            var person = new Accounts();

            person.Username = create.Username;
            person.EmailAddress = create.EmailAddress;
            _passwordLogic.CreateHashPassword(create.Password, out byte[] passwordHash, out byte[] PasswordSalt);
            person.PasswordHash = passwordHash;
            person.PasswordSalt = PasswordSalt;

            var execution = await _accountRepo.CreateAccount(person);
            if (execution == null)
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
            return new HttpStatusResult()
            {
                StatusCode = HttpStatusCode.OK,
                Message = execution.ToAccountDTO()
            };


        }

        public async Task<HttpStatusResult> DeleteAccount(LoginDTO login)
        {
            try
            {
                await _accountRepo.DeleteAccountFromRepo(login);
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = new List<string>()
                    {
                        "Response: success",
                        $"Message: Your account has been deleted",
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
                        $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                        $"Message: {ex.Message}"
                    }
                };
            }
        }

        public async Task<HttpStatusResult> ForgotPassword(string Email)
        {
            try
            {
                var confirmedUser = await _accountRepo.GetbyEmail(Email);
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

                //current smtp connection is for gmail
                //for future implementation using an if statement to check contents of email (gmail,yahoo,hotmail etc..) and sent using their respective smtp connection

                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate("curtisdave86@gmail.com", "kejq gqjg ymfd ziwl");
                smtp.Send(email);
                smtp.Disconnect(true);

                await _accountRepo.SaveChangesToDB(confirmedUser);
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
                    $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                    $"Message: {ex.Message}"
                    }
                };
            }
        }

        public async Task<HttpStatusResult> GetAccountById(int id)
        {

            try
            {
                var result = await _accountRepo.GetAccountById(id);
                if (result == null)
                {
                    return new HttpStatusResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = new List<string>
                    {
                        "Response: Not Found",
                        $"ErrorCode: {(int)HttpStatusCode.NotFound}",
                        "Message: Account Not Found"
                    },
                    };
                }
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.ToAccountDTO(),

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
                        $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                        $"Message: {ex.Message}"
                    }
                };
            }
           
        }

        public HttpStatusResult GetAllAccounts()
        {
            var All_accounts = _accountRepo.GetAllAccounts();
            if (All_accounts.Count == 0)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = new List<string>
                    {
                        "Response: Not Found",
                        "ErrorCode: 404",
                        "Message: Accountx Not Found"
                    },
                };
            }
            return new HttpStatusResult()
            {
                StatusCode = HttpStatusCode.OK,
                Message = All_accounts
            };
        }

        public async Task<HttpStatusResult> Login(LoginDTO login)
        {
            try
            {
                var user = await _accountRepo.GetLoggedInAccount(login);
                var VerifyPassword = _passwordLogic.VerifyPassword(login.Password, user.PasswordHash, user.PasswordSalt);
                if (!VerifyPassword)
                {
                    throw new Exception("Incorrect password");
                }

                var jwt = await _jwtGenerator.CreateToken(user);
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = new List<string>
                    {
                    "Message: Success",
                    $"Token: {jwt}"
                    }
                };
            }
            catch (Exception ex)
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
    }
}
