using CleanAuthenticator.Application.DTOs.Stock;
using CleanAuthenticator.Application.Interfaces.Stock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CleanAuthenticator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }
        // get stocks by Id
       
        [HttpGet("Get stock by {id:int}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var stockkk= await _stockService.GetstockbyId(id);
            return StatusCode((int)stockkk.StatusCode,stockkk.Message);
        }


        //get all stocks
        [HttpGet]
        [Authorize]
        [Route("Get all Stocks")]

        public async Task<IActionResult> GetAllStocks()
        {
            var response = await _stockService.GetAllStocks();
            return StatusCode((int)response.StatusCode, response.Message);
        }

        //get stock by CompanyName

        [HttpGet]
        [Authorize]
        [Route("Search Stock using companyName")]

        public async Task<IActionResult>Search(string CompanyName)
        {
            var response = await _stockService.SearchStock(CompanyName);
            return StatusCode((int)response.StatusCode,response.Message);
        }

        //edit stock info
        [HttpPut]
        [Route("Update Stock Using StockId")]

        public async Task<IActionResult>UpdateStockInformation(int StockId, UpdateStockDTO update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if DTO is not valid
            }
            var response = await _stockService.EditStockInfo(StockId, update);
            return StatusCode((int)response.StatusCode, response.Message);
        }

        // create a stock 
        [HttpPost]
        [Route("Create Stock")]

       public async Task<IActionResult> CreateStock(CreateStockDTO create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _stockService.CreateStock(create);
            return StatusCode((int)response.StatusCode, response.Message);
        }

       

        // DELETE stock

        [HttpDelete]
        [Authorize]
        [Route("Delete stock")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var response = await _stockService.DeleteStock(id);
            return StatusCode((int)response.StatusCode, response.Message);
        }
    }
}
