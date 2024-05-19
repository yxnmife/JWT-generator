using Authenticator.Caching;
using Authenticator.Controllers;
using Authenticator.Data;
using Authenticator.DTO.Portfolio;
using Authenticator.Extensions;
using Authenticator.Interfaces;
using Authenticator.Mappers;
using Authenticator.Models;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace Authenticator.Repository.Port
{
    public class PortfolioRepo:IPortfolioRepo
    {

        private readonly ApplicationDbContext _db;
        private readonly ICacheProvider _cache;
        private readonly ILogger<PortfoliosController> _logger;
       
        public PortfolioRepo(ApplicationDbContext db,ILogger<PortfoliosController> logger,ICacheProvider cache)
        {
            _db = db;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Portfolio> CreatePortfolio(int StockId)
        {
            var newPortfolio = new Portfolio();
            var stock = await _db.StockTable.FirstOrDefaultAsync(x => x.Id == StockId);
            if (stock == null)
            {
                return null;
            }
            newPortfolio.StockId = stock.Id;
            return newPortfolio;
        }

        public async Task<List<PortfolioDTO>> GetAllPortfolio(int AccountId)
        {
            var portfolios =  _db.PortfolioTable
                .Where(acc=>acc.AccountId==AccountId)
                .Select(acc=>acc.ToPortfolioDTO())              
                .ToList();
            if (portfolios.Count == 0)
            {
                return null;
            }
            return portfolios;
        }
        public async Task<Portfolio> GetPortfolio(int StockId)
        {
            var portFolio= _db.PortfolioTable.FirstOrDefaultAsync(x=>x.StockId == StockId);
            if(portFolio == null)
            {
                return null;
            }
            return await portFolio;
        }
        public async Task<HttpStatusResult> PortfolioCreationValidation(Portfolio? portFolio,string Id)
        {
            if (portFolio == null)
            {
                _logger.LogError($"{DateTime.Now}: Failed to create Portfolio");
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = new List<string>
                    {
                         "Response: Bad Request",
                        "ErrorCode: 400",
                        "Message: Failed to create Portfolio"
                    }
                };
            }
            portFolio.AccountId = Convert.ToInt32(Id);
            _logger.LogInformation($"{DateTime.Now}: Portfolio created successfully");
            await _db.PortfolioTable.AddAsync(portFolio);
            await _db.SaveChangesAsync();
            return new HttpStatusResult()
            {
                StatusCode = HttpStatusCode.OK,
                Message = portFolio.ToPortfolioDTO()
            };
        }
        public async Task<HttpStatusResult>GetPortfoliofromCache(int AccountId)
        {
            if (!_cache.TryGetValue(CacheKeys.UserPortfolio + AccountId, out List<PortfolioDTO> portFolios))
            {
                portFolios = await GetAllPortfolio(AccountId);
                if (portFolios == null)
                {
                    _logger.LogError($"{DateTime.Now}: User Portfolio Not found");
                    return new HttpStatusResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = new List<string>
                        {
                            "Response: Not Found",
                            "ErrorCode: 404",
                            "Message: User Portfolio not found"
                        }
                    };
                }
                var CacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(2),
                    SlidingExpiration = TimeSpan.FromMinutes(1)
                };
                _cache.Set(CacheKeys.UserPortfolio + AccountId, portFolios, CacheEntryOptions);
                _logger.LogInformation($"{DateTime.Now}: User Portfolio Returned");
            }
            return new HttpStatusResult()
            {
                StatusCode = HttpStatusCode.OK,
                Message = portFolios
            };
        }
        public async Task<HttpStatusResult> PortfolioDeletionValidation(Portfolio portfolio)
        {
            if (portfolio == null)
            {
                _logger.LogError($"{DateTime.Now}: User Portfolio Not found");
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = new List<string>()
                    {
                        "Response: Not Found",
                        "ErrorCode: 404",
                        "Message: User Portfolio not found"
                    }
                };
            }
            else
            {
                _db.PortfolioTable.Remove(portfolio);
                await _db.SaveChangesAsync();
                _logger.LogInformation($"{DateTime.Now}: User Portfolio deleted successfully");
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = portfolio.ToPortfolioDTO()
                };
            }
        }

    }
}
