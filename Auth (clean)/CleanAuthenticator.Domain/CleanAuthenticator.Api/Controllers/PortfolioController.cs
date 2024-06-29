using CleanAuthenticator.Application.Interfaces.Port;
using CleanAuthenticator.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAuthenticator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        public PortfolioController(IPortfolioService portfolioService)
        {
             _portfolioService = portfolioService;
        }
        // get Portfolio
        [HttpGet]
        [Authorize]
        [Route("Get all user Portfolios")]
        public async Task<IActionResult> GetPortfolio()
        {
            var userId = User.GetID();
            var response =  await _portfolioService.GetAllPortfolio(userId);
            return StatusCode((int)response.StatusCode, response.Message);
        }



        // Create Portfolio
        [HttpPost]
        [Authorize]
        [Route("Create Portfolio")]
        public async Task<IActionResult> Create(int StockId)
        {
            var userId = User.GetID();
            var response = await _portfolioService.CreatePortfolio(userId, StockId);
            return StatusCode((int)response.StatusCode, response.Message);
        }

        

        // DELETE user portfolios
        [HttpDelete]
        [Authorize]
        [Route("Remove stock from User Portfolio")]
        
        public async Task<IActionResult> RemoveStock(int StockID)
        {
            var userId = User.GetID();
            var response= await _portfolioService.RemoveStockfromPortfolio(userId, StockID);
            return StatusCode((int)response.StatusCode, response.Message);
        }
    }
}
