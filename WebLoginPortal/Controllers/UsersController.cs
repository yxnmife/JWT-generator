using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebLoginPortal.Data;
using WebLoginPortal.DTO;
using WebLoginPortal.Mappers;
using WebLoginPortal.Models;

namespace WebLoginPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly IPasswordLogic _passwordLogic;
       
        public UsersController(ApplicationDbContext db, IConfiguration config,IPasswordLogic passwordLogic)
        {
            _db = db;
            _config = config;
            _passwordLogic = passwordLogic;
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser(CreateUserDTO user)
        {
            var data = new UserInfo();
            data.Email= user.Email;
            data.FirstName=user.FirstName;
            data.LastName = user.LastName;
         
            var Userexists= await _db.UsersTable.AnyAsync(c=> c.Email.ToLower()==data.Email.ToLower());//This checks the database to check if an existed user exists with the same email.
            if (Userexists)
            {
                return BadRequest("Email address already in Database");
            }
            _passwordLogic.CreateHashPassword(user.Password, out byte[] passwordHash, out byte[] PasswordSalt);
            data.PasswordHash=passwordHash;
            data.PasswordSalt = PasswordSalt;

           await _db.UsersTable.AddAsync(data);
           await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = data.Id }, data.ToUserInfoDTO());
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO logindetails)
        {
            var email = logindetails.Email.ToLower(); // Convert email to lowercase for comparison
            var user = await _db.UsersTable.FirstOrDefaultAsync(x => x.Email.ToLower() == email);

            if (user == null)
            {
                return NotFound("User not found in database");
            }

            if (!_passwordLogic.VerifyPassword(logindetails.Password,user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong Password");
            }

            string token = CreateToken(user);
            return Ok(token);
        }


        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var data = await _db.UsersTable.FirstOrDefaultAsync(x=>x.Id == id);
            if (data == null)
            {
                return NotFound("User not found");
            }
            
            return Ok(data.ToUserInfoDTO());
        }
        [AllowAnonymous]
        [HttpGet("All Users")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _db.UsersTable.Select(x => x.ToUserInfoDTO()).ToListAsync();
            return Ok(data);
        }
        [HttpDelete]
        public async Task<IActionResult>RemoveUser(int id)
        {
            var user=await _db.UsersTable.FirstOrDefaultAsync(x=>x.Id==id);
            if (user == null)
            {
                return NotFound("User not found");
            }
           _db.UsersTable.Remove(user);
           await _db.SaveChangesAsync();
            return Ok("User removed successfully");

        }

        private string CreateToken(UserInfo user)
        {
          
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            List<Claim> ClaimsList = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.FirstName),
                new Claim(ClaimTypes.Name,user.LastName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims:ClaimsList,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
                );
            var jwt= new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }



    }
}
