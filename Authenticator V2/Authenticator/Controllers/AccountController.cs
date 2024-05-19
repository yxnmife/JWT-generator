using Authenticator.Data;
using Authenticator.DTO.Account;
using Authenticator.Extensions;
using Authenticator.Interfaces;
using Authenticator.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Authenticator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccount _accountrepo;
       

        public AccountController(IAccount createAccount)
        {
            _accountrepo = createAccount;
        }

        [HttpGet]
        [Route("Get By UserId")]
        public async Task<IActionResult> GetById(int id)
        {
            var person=await _accountrepo.GetAccountFromCacheOrRepo(id);
            var status = await _accountrepo.AccountNonExistent(person);
            return StatusCode((int)status.StatusCode,(status.Message));
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> CreateAccount(CreateAccountDTO create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if DTO is not valid
            }
            var data = await _accountrepo.Create(create);
            var response = await _accountrepo.CreateAccountValidation(data);
           
            return CreatedAtAction(nameof(GetById), new { id = data.Id }, response.Message);
        }

        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login(LoginDTO login)
        {
            var user= await _accountrepo.Login(login);
            var response = await _accountrepo.LoginValidation(user);

            return StatusCode((int)(response.StatusCode),response.Message);
        }

        [HttpPost]
        [Authorize]
        [Route("Change Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO change)
        {
            var user = User.GetID();
            var Id = Convert.ToInt32(user);
            var userAccount = await _accountrepo.GetAccountFromCacheOrRepo(Id);
            var data = await _accountrepo.ChangePassword(userAccount, change);
            var response=await _accountrepo.ChangePasswordValidation(data);
          
            return StatusCode((int)response.StatusCode, response.Message);
        }

      [HttpDelete]

        [Route("Delete Users")]
        public async Task<IActionResult> Delete(LoginDTO login)
        {
            var user = await _accountrepo.FindAccount(login);
            var response= await _accountrepo.DeleteAccountValidation(user);

            return StatusCode((int)response.StatusCode, response.Message);
        }
    }
}
