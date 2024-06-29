using CleanAuthenticator.Application.DTOs.Account;
using CleanAuthenticator.Application.Interfaces.Account;
using CleanAuthenticator.Application.Services;
using CleanAuthenticator.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAuthenticator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        // get account by Id
        [HttpGet]
        [Route("GetAccountById")]
        public async Task<IActionResult> GetById(int id)
        {
            var person = await _accountService.GetAccountById(id);
            return StatusCode((int)person.StatusCode, (person.Message));
        }

        // get all accounts
        [HttpGet]
        [Route("Get All Accounts")]
        public IActionResult AllAccounts()
        {
            var person = _accountService.GetAllAccounts();
            return StatusCode((int)person.StatusCode, (person.Message));
        }

        // create user account
        [HttpPost]
        [Route("Create an Account")]
        public async Task<IActionResult> CreateAccountAsync(CreateAccountDTO create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if DTO is not valid
            }
            var response = await _accountService.CreateAccount(create);
            return StatusCode((int)response.StatusCode, response.Message);
        }

        //account Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> AccountLogin(LoginDTO login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if DTO is not valid
            }
            var response = await _accountService.Login(login);
            return StatusCode((int)(response.StatusCode), response.Message);
        }

        //Change account password
        [HttpPost]
        [Authorize]
        [Route("Change account Password")]
        public async Task<IActionResult> CHangePassword(ChangePasswordDTO change)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if DTO is not valid
            }
            var userId = User.GetID();
            var response = await _accountService.ChangePassword(userId, change);
            return StatusCode((int)(response.StatusCode), response.Message);

        }

        //delete a user
        [HttpDelete]
        [Route("Delete user account")]
        public async Task<IActionResult> DeleteAccount(LoginDTO login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if DTO is not valid
            }
            var response = await _accountService.DeleteAccount(login);
            return StatusCode((int)(response.StatusCode), response.Message);
        }

        //reset account password
        [HttpGet]
        [Route("Reset account password")]

        public async Task<IActionResult> ForgotPassword(string email)
        {
            var Response = await _accountService.ForgotPassword(email);
            return StatusCode((int)Response.StatusCode, Response.Message);
        }

    }
}
