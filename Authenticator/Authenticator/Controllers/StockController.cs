using Authenticator.Caching;
using Authenticator.Data;
using Authenticator.DTO.Stock;
using Authenticator.Interfaces;
using Authenticator.Mappers;
using Authenticator.Models;
using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Authenticator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IStockRepo _stockRepo;
        private readonly IAccount _accountrepo;
        private readonly ICacheProvider _cache;

        public StockController(ApplicationDbContext db, IAccount createAccount,IStockRepo stockRepo,ICacheProvider cache)
        {
            _db = db;
            _accountrepo = createAccount;
            _stockRepo = stockRepo;
            _cache = cache;
        }
        [HttpGet]
        [Route("Get By StockId")]
        public async Task<IActionResult> GetById(int id)
        {
            if(!_cache.TryGetValue(CacheKeys.GetStock+id,out Stocks? stocks))
            {
                stocks = await _db.StockTable.FindAsync(id);
                if (stocks == null)
                {
                    return NotFound("Stock does not exist");
                }
                var CacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(2),
                    SlidingExpiration = TimeSpan.FromMinutes(1),
                    Priority=CacheItemPriority.Normal
                };
                _cache.Set(CacheKeys.GetStock + id, stocks, CacheEntryOptions);
            }
            return Ok(stocks.ToStockDTO());
        }
        [HttpGet]
        [Authorize]
        [Route("Get all stocks")]

        public async Task<IActionResult> GetAll()
        {
            if(!_cache.TryGetValue(CacheKeys.GetStock, out List<StockDTO> stocks))
            {
                stocks = await _stockRepo.GetAll();
                if (stocks == null)
                {
                    return NotFound("No stocks found");
                }

                var CacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration=DateTime.Now.AddMinutes(2),
                };
                _cache.Set(CacheKeys.GetStock, stocks, CacheEntryOptions);  
            }
            return Ok(stocks);
        }


        [HttpPost]
        [Route("Create Stock")]
        
        public async Task<IActionResult> Create(CreateStockDTO create)
        {
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var NewStock = new Stocks();
            
           var status= await _stockRepo.CreateStocks(create, NewStock);
            if (status == null)
            {
                return BadRequest("stock already exists in db");
            }
           
            return CreatedAtAction(nameof(GetById), new { id = NewStock.Id}, NewStock.ToStockDTO());
        }

        [HttpPost]
        [Route("Edit Stock using StockId")]

        public async Task<IActionResult> Edit(int id,UpdateStockDTO update)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            var StockModel= await _stockRepo.GetStockbyID(id);

            if(StockModel== null)
            {
                return NotFound("Stock not found");
            }
           var data = await _stockRepo.EditStock(update);

            return Ok(data.ToStockDTO());
        }
        [HttpGet]
        [Route("Edit stock using CompanyName")]
        public async Task<IActionResult> EditByName( UpdateStockDTO update, string name)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var stockmodel = await _stockRepo.GetByName(name);

            if (stockmodel == null)
            {
                return NotFound("Stock not found");
            }
            var data = await _stockRepo.EditStock(update);

            return Ok(data.ToStockDTO());
        }

        [HttpDelete]
        [Route("Delete Stock")]
        public async Task<IActionResult> Delete(int id)
        {
            var stocks = await _db.StockTable.FindAsync(id);
            if (stocks == null)
            {
                return NotFound("Stock does not exist");
            }
             _db.StockTable.Remove(stocks);
            await _db.SaveChangesAsync();
            return Ok(stocks);
        }
    }
}
