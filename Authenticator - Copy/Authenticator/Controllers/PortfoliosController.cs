using Authenticator.Data;
using Authenticator.Extensions;
using Authenticator.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfoliosController : ControllerBase
    {
       
        private readonly IPortfolioRepo _portfoliorepo;
        private readonly ApplicationDbContext _db;
      
        public PortfoliosController(IPortfolioRepo portfolioRepo, ApplicationDbContext db)
        {
            _db = db;
            _portfoliorepo = portfolioRepo;
        }


        [HttpPost]
        [Authorize]
        [Route("Create Portfolio")]

        public async Task<IActionResult> CreatePortfolio(int stockId)
        {
            var userId=User.GetID();
                      
            var response = await _portfoliorepo.PortfolioCreationValidation(stockId,userId);
          
            return StatusCode((int)response.StatusCode,response.Message);
        }

        [HttpGet]
        [Authorize]
        [Route("Get user portfolio")]

        public async Task<IActionResult> GetPortfolio()
        {
            var userId=User.GetID();
            var AccountId=Convert.ToInt32(userId);
            var response = await _portfoliorepo.GetPortfoliofromCache(AccountId);
            return StatusCode((int)response.StatusCode,response.Message); 

        }

        [HttpDelete]
        [Authorize]
        [Route("Delete Portfolio")]

        public async Task<IActionResult> DeletePortfolio(int stockId)
        {
            var userId= User.GetID();
            var porTfolio= await _portfoliorepo.GetPortfolio(stockId);
            var response = await _portfoliorepo.PortfolioDeletionValidation(porTfolio);

            return StatusCode((int)response.StatusCode, response.Message);
        }

    }
}
