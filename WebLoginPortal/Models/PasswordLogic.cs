using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebLoginPortal.Data;
using WebLoginPortal.DTO;
using WebLoginPortal.Mappers;

namespace WebLoginPortal.Models
{
    public class PasswordLogic : IPasswordLogic
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        public PasswordLogic(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
        public void CreateHashPassword(string password,out byte[] passwordHash,out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())

            {
                passwordSalt = hmac.Key;

                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                
            }
        }
        public bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedhash.SequenceEqual(passwordHash);
            }
        }

        public byte[] HashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())

            {
                passwordSalt = hmac.Key;

                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            }
            return passwordHash;
        }
       
        public async Task<UserInfo?> CreateUser(CreateUserDTO user)
        {
            var data = new UserInfo();
            data.Email = user.Email;
            data.FirstName = user.FirstName;
            data.LastName = user.LastName;

            var Userexists = await _db.UsersTable.AnyAsync(c => c.Email.ToLower() == data.Email.ToLower());//This checks the database to check if an existed user exists with the same email.
            if (Userexists)
            {
                return null;
            }
            CreateHashPassword(user.Password, out byte[] passwordHash, out byte[] PasswordSalt);
            data.PasswordHash = passwordHash;
            data.PasswordSalt = PasswordSalt;
            return data;
        }
        public async Task<string>CreateToken(UserInfo user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            List<Claim> ClaimsList = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.FirstName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims: ClaimsList,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        public async Task<UserInfo?> CheckLoginDetails(LoginDTO logindetails)
        {
            var email = logindetails.Email.ToLower(); // Convert email to lowercase for comparison
            var user = await _db.UsersTable.FirstOrDefaultAsync(x => x.Email.ToLower() == email);
            return user;
        }
        public async Task<UserInfo?> CheckIDDetails(int id)
        {
            var user = await _db.UsersTable.FirstOrDefaultAsync(x => x.Id==id);
            return user;
        }
        public async Task<List<UserInfoDTO?>> GetAllUsers()
        {
            var data = await _db.UsersTable.Select(x => x.ToUserInfoDTO()).ToListAsync();
            return data;
        }

    }
}
