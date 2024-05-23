using Authenticator.Caching;
using Authenticator.Data;
using Authenticator.DTO.Stock;
using Authenticator.Extensions;
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
        private readonly ICacheProvider _cache;
        private readonly ILogger<StockController> _logger;

        public StockController(ApplicationDbContext db,IStockRepo stockRepo,ICacheProvider cache, ILogger<StockController> logger)
        {
            _db = db;
            _stockRepo = stockRepo;
            _cache = cache;
            _logger=logger;
        }
        [HttpGet]
        [Route("Get By StockId")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _stockRepo.GetbyIdfromCache(id);

            return StatusCode((int)response.StatusCode,response.Message);
        }
        [HttpGet]
        [Authorize]
        [Route("Get all stocks")]

        public async Task<IActionResult> GetAll()
        {
            var response = await _stockRepo.GetAllStocksValidation();
            return StatusCode((int)response.StatusCode, response.Message);
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
            
           var stock= await _stockRepo.CreateStocks(create);
            if (stock == null)
            {
                var response = _stockRepo.StockExistsResponse();
                return StatusCode((int)response.StatusCode, response.Message);
            }
           
            return CreatedAtAction(nameof(GetById), new { id = stock.Id}, NewStock.ToStockDTO());
        }

        [HttpPost]
        [Route("Edit Stock using StockId")]

        public async Task<IActionResult> Edit(int id,UpdateStockDTO update)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            var response= await _stockRepo.EditStockbyIDValidation(id, update);
            return StatusCode((int)response.StatusCode, response.Message);
        }
        [HttpGet]
        [Route("Edit stock using CompanyName")]
        public async Task<IActionResult> EditByName( UpdateStockDTO update, string name)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
         var response= await _stockRepo.EditStockbyNameValidation(update, name);
            return StatusCode((int)response.StatusCode, response.Message);
        }

        [HttpDelete]
        [Route("Delete Stock")]
        public async Task<IActionResult> Delete(int id)
        {
            var stocks = await _db.StockTable.FindAsync(id);
            var response= await _stockRepo.StockDeleteValidation(stocks);
           
            return StatusCode((int)response.StatusCode,response.Message);
        }
    }
}
