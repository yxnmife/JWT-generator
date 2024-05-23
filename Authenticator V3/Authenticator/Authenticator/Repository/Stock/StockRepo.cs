using Authenticator.Caching;
using Authenticator.Controllers;
using Authenticator.Data;
using Authenticator.DTO.Stock;
using Authenticator.Extensions;
using Authenticator.Interfaces;
using Authenticator.Mappers;
using Authenticator.Models;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Authenticator.Repository.Stock
{
    public class StockRepo:IStockRepo
    {
        private readonly ApplicationDbContext _db;
        private readonly ICacheProvider _cache;
        private readonly ILogger<StockController> _logger;
        public StockRepo(ApplicationDbContext db, ICacheProvider cache, ILogger<StockController> logger)
        {
            _db= db;
            _cache= cache;
            _logger= logger;
        }
        public async Task<Stocks> CreateStocks(CreateStockDTO createstock)
        {
            var stockmodel = new Stocks();
            var isExisting = await _db.StockTable.AnyAsync(x=>x.CompanyName.ToLower()==createstock.CompanyName.ToLower()|| x.Symbol.ToLower() == createstock.Symbol.ToLower());
            if(isExisting)
            {
                throw new DuplicateNameException();
            }

            stockmodel.Symbol = createstock.Symbol;
            stockmodel.CompanyName = createstock.CompanyName;
            stockmodel.Purchase = createstock.Purchase;
            stockmodel.Div = createstock.LastDiv;
            stockmodel.Industry = createstock.Industry;
            stockmodel.MarketCap = createstock.MarketCap;
            await _db.StockTable.AddAsync(stockmodel);
            await _db.SaveChangesAsync();
            return stockmodel;
        }

        private async Task<Stocks> EditStock(UpdateStockDTO updatestock)
        {
            var stockmodel = new Stocks();

            stockmodel.Symbol = updatestock.Symbol;
            stockmodel.CompanyName = updatestock.CompanyName;
            stockmodel.Purchase = updatestock.Purchase;
            stockmodel.Div = updatestock.LastDiv;
            stockmodel.Industry = updatestock.Industry;
            stockmodel.MarketCap = updatestock.MarketCap;

            await _db.SaveChangesAsync();
            return stockmodel;
        }

       public HttpStatusResult StockExistsResponse()
        {
            return new HttpStatusResult()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = new List<string>()
                {
                    "Response: Bad Request",
                    $"ErrorCode:{(int)HttpStatusCode.BadRequest} ",
                    "Message: Stock already exists"
                }
            };
        }

        public async Task<HttpStatusResult> GetAllStocksValidation()
        {

            if (!_cache.TryGetValue(CacheKeys.GetStock, out List<StockDTO> stocks))
            {
                stocks = await GetAll();
                if (stocks == null)
                {
                    return new HttpStatusResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = new List<string>()
                        {
                        "Response: Not Found",
                        $"ErrorCode: {(int)HttpStatusCode.NotFound}",
                        "Message: No stock found"
                        }
                    };
                }

                var CacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(2),
                };
                _cache.Set(CacheKeys.GetStock, stocks, CacheEntryOptions);
            }
            return new HttpStatusResult()
            {
                StatusCode=HttpStatusCode.OK,
                Message=stocks
            };
        }

        public async Task<HttpStatusResult> GetbyIdfromCache(int id)
        {
            if (!_cache.TryGetValue(CacheKeys.GetStock + id, out Stocks? stocks))
            {
                stocks = await _db.StockTable.FindAsync(id);
                if (stocks == null)
                {
                    _logger.LogWarning($"{DateTime.Now}: stocks for customer {id} not found ");
                    return new HttpStatusResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = new List<string>()
                        {
                        "Response: Not Found",
                        "ErrorCode: 404",
                        "Message: Stock does not exist"
                        }
                    };
                    
                }
                var CacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(2),
                    SlidingExpiration = TimeSpan.FromSeconds(10)
                };
                _logger.LogInformation($"{DateTime.Now}: stocks for customer {id} retrieved");
                _cache.Set(CacheKeys.GetStock + id, stocks, CacheEntryOptions);
            }
            return new HttpStatusResult()
            {
                StatusCode = HttpStatusCode.OK,
                Message = stocks.ToStockDTO()
            };
        }

        public async Task<HttpStatusResult> EditStockbyIDValidation(int id, UpdateStockDTO update)
        {
            var StockModel = await GetStockbyID(id);

            if (StockModel == null)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = new List<string>()
                    {
                        "Response: Not Found",
                        "ErrorCode: 404",
                        "Message: Stock does not exist"
                    }
                };
            }
            var data = await EditStock(update);
            return new HttpStatusResult()
            {
                StatusCode = HttpStatusCode.OK,
                Message = data.ToStockDTO()
            };

        }

        public async Task<HttpStatusResult> EditStockbyNameValidation(UpdateStockDTO update, string name)
        {
            var stockmodel = await GetByName(name);

            if (stockmodel == null)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = new List<string>()
                    {
                         "Response: Not Found",
                        "ErrorCode: 404",
                        "Message: Stock does not exist"
                    }
                };
            }
            var data = await EditStock(update);
            return new HttpStatusResult()
            {
                StatusCode = HttpStatusCode.OK,
                Message = data.ToStockDTO()
            };

        }

        public async Task<HttpStatusResult> StockDeleteValidation(Stocks stocks)
        {
            if (stocks == null)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = new List<string>()
                    {
                         "Response: Not Found",
                        "ErrorCode: 404",
                        "Message: Stock does not exist"
                    }
                };

            }
            _db.StockTable.Remove(stocks);
            await _db.SaveChangesAsync();
            return new HttpStatusResult()
            {
                StatusCode = HttpStatusCode.OK,
                Message = stocks.ToStockDTO()
            };
        }
        
        private async Task<List<StockDTO>> GetAll()
        {
            var items = await _db.StockTable.Select(x=>x.ToStockDTO()).ToListAsync();
            return items;
        }
        private async Task<Stocks>GetStockbyID(int id)
        {
            var stock = await _db.StockTable.FirstOrDefaultAsync(x => x.Id == id);
            if (stock == null)
            {
                return null;
            }

            return stock;
        }
        private async Task<Stocks>GetByName(string name)
        {
            var stock = await _db.StockTable.FirstOrDefaultAsync(x => x.CompanyName.Contains(name));
            if (stock == null)
            {
                return null;
            }

            return stock;
        }
    }

}
