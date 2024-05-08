using Authenticator.Caching;
using Authenticator.Data;
using Authenticator.DTO.Portfolio;
using Authenticator.Extensions;
using Authenticator.Interfaces;
using Authenticator.Mappers;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Authenticator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfoliosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IPortfolioRepo _portfoliorepo;
        private readonly ICacheProvider _cache;

        public PortfoliosController(ApplicationDbContext db,IPortfolioRepo portfolioRepo, ICacheProvider cache)
        {
            _db = db; 
            _portfoliorepo = portfolioRepo;
            _cache = cache;
        }


        [HttpPost]
        [Authorize]
        [Route("Create Portfolio")]

        public async Task<IActionResult> CreatePortfolio(int stockId)
        {
            var userId=User.GetID();
            var portFolio=await _portfoliorepo.CreatePortfolio(stockId);
            if (portFolio == null)
            {
                return BadRequest("Operation Failed");
            }
            portFolio.AccountId=Convert.ToInt32(userId);
            await _db.PortfolioTable.AddAsync(portFolio);
            await _db.SaveChangesAsync();
            return Ok(portFolio.ToPortfolioDTO());
        }

        [HttpGet]
        [Authorize]
        [Route("Get user portfolio")]

        public async Task<IActionResult> GetPortfolio()
        {
            var userId=User.GetID();
            var AccountId=Convert.ToInt32(userId);
            if(!_cache.TryGetValue(CacheKeys.UserPortfolio+AccountId, out List<PortfolioDTO> portFolios))
            {
                portFolios = await _portfoliorepo.GetAllPortfolio(AccountId);
                if (portFolios == null)
                {
                    return NotFound("User does not have any Portfolio");
                }
                var CacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(2),
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                };
                _cache.Set(CacheKeys.UserPortfolio + AccountId, portFolios, CacheEntryOptions);
            }
            return Ok(portFolios);

        }

        [HttpDelete]
        [Authorize]
        [Route("Delete Portfolio")]

        public async Task<IActionResult> DeletePortfolio(int stockId)
        {
            var userId= User.GetID();
            var porTfolio= await _portfoliorepo.GetPortfolio(stockId);
            if(porTfolio == null)
            {
                return NotFound("Portfolio does not exist");
            }

            _db.PortfolioTable.Remove(porTfolio);
           await _db.SaveChangesAsync();
            return Ok(porTfolio.ToPortfolioDTO());
        }
    }
}
