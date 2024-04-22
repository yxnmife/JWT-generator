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
using WebLoginPortal.Helpers;
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
            var data= await _passwordLogic.CreateUser(user);
            if (data==null)
            {
                return BadRequest("Email already exists in database");
            }

           await _db.UsersTable.AddAsync(data);
           await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = data.Id }, data.ToUserInfoDTO());
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO logindetails)
        {
            var user = await _passwordLogic.CheckLoginDetails(logindetails);

            if (user == null)
            {
                return NotFound("User not found in database");
            }
            var Verify = _passwordLogic.VerifyPassword(logindetails.Password, user.PasswordHash, user.PasswordSalt);
            if (!Verify)
            {
                return BadRequest("Wrong Password");
            }
            var Token=_passwordLogic.CreateToken(user);
            return Ok(Token);
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
            var data = await _passwordLogic.GetAllUsers();
            return Ok(data);
        }
        [HttpDelete]
        public async Task<IActionResult>RemoveUser(int id)
        {
            var user= await _passwordLogic.CheckIDDetails(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
           _db.UsersTable.Remove(user);
           await _db.SaveChangesAsync();
            return Ok(user);

        }

        [HttpGet]
        [Authorize]
        [Route("Display owner of token being used")]
        public async Task<IActionResult> ReturnDetails()
        {
            var username = User.GetUsername();
            var person = await _db.UsersTable.FirstOrDefaultAsync(x => x.FirstName == username);
            var Details = person.LastName;

            return Ok("Welcome "+Details+"!");
        }
    }
}
