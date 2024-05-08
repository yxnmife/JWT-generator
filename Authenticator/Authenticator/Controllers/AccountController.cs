using Authenticator.Caching;
using Authenticator.Data;
using Authenticator.DTO.Account;
using Authenticator.Extensions;
using Authenticator.Interfaces;
using Authenticator.Mappers;
using Authenticator.Models;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;


namespace Authenticator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IAccount _accountrepo;
        private readonly ICacheProvider _cache;

        public AccountController(ApplicationDbContext db,IAccount createAccount,ICacheProvider cache)
        {
            _db = db;
            _accountrepo = createAccount;
            _cache = cache;
        }

        [HttpGet]
        [Route("Get By UserId")]
        public async Task<IActionResult> GetById(int id)
        {
            if(!_cache.TryGetValue(CacheKeys.GetAccount+id, out Accounts person))
            {
                person = await _accountrepo.GetAccount(id);
                if (person == null)
                {
                    return NotFound("Account not found");
                }
                var CacheEntryOption = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(1),
                    SlidingExpiration=TimeSpan.FromSeconds(30)
                };
                _cache.Set(CacheKeys.GetAccount+id, person, CacheEntryOption);
            }
            return Ok(person.ToAccountDTO());

        }

        [HttpPost]
       
        [Route("Register")]
       
        public async Task<IActionResult> CreateAccount(CreateAccountDTO create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if DTO is not valid
            }

            var person = new Accounts();
            var data = await _accountrepo.Create(person, create);
            if(data == null)
            {
                return BadRequest("User already exists in db");
            }
            await _db.AccountsTable.AddAsync(data);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = data.Id }, data.ToAccountDTO());
        }

        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login(LoginDTO login)
        {
            var user= await _accountrepo.Login(login);
            if (user == null)
            {
                return BadRequest("User not found or Incorrect Password");
            }
            return Ok(user);
        }
        [HttpPost]
        [Authorize]
        [Route("Change Password")]

        public async Task<IActionResult> ChangePassword(ChangePasswordDTO change)
        {
            var user = User.GetID();
            var Id = Convert.ToInt32(user);
            if (!_cache.TryGetValue(CacheKeys.GetAccount + Id, out Accounts userAccount))
            {
                userAccount = await _accountrepo.FindAccountwithId(Id);
                var CacheEntryOption = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(1),
                    SlidingExpiration = TimeSpan.FromSeconds(30)
                };
                _cache.Set(CacheKeys.GetAccount +Id, userAccount, CacheEntryOption);
            }
              
            var data = await _accountrepo.ChangePassword(userAccount, change);
            if(data == null)
            {
                return BadRequest("Passwords do not match");
            }
           await _db.SaveChangesAsync();
            return Ok(data.ToAccountDTO());
        }



      [HttpDelete]

        [Route("Delete Users")]
        public async Task<IActionResult> Delete(LoginDTO login)
        {
            var user = await _accountrepo.FindAccount(login);
            if (user == null)
            {
                return NotFound("user not found");
            }
            if (user == default)
            {
                return BadRequest("Incorrect Password");
            }
            _db.AccountsTable.Remove(user);
            await _db.SaveChangesAsync();
            return Ok(user.ToAccountDTO());
        }
   

    }
}
