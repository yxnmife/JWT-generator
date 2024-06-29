using CleanAuthenticator.Application.DTOs.Stock;
using CleanAuthenticator.Application.Interfaces.Stock;
using CleanAuthenticator.Domain;
using System.Net;
using CleanAuthenticator.Application.Mappers;
namespace CleanAuthenticator.Application.Services.Stock

{
    public class StockService : IStockService
    {
        private readonly IStockRepo _stockRepo;
        public StockService(IStockRepo stockRepo)
        {
            _stockRepo = stockRepo;
        }
        public async Task<HttpStatusResult> CreateStock(CreateStockDTO createstock)
        {
            try
            {
                var stockmodel = new Stocks();


                stockmodel.Symbol = createstock.Symbol;
                stockmodel.CompanyName = createstock.CompanyName;
                stockmodel.Purchase = createstock.Purchase;
                stockmodel.Div = createstock.LastDiv;
                stockmodel.Industry = createstock.Industry;
                stockmodel.MarketCap = createstock.MarketCap;
                var NewStock = await _stockRepo.CreateStocks(stockmodel);

                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = NewStock.ToStockDTO()
                };
            }
            catch( Exception ex)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = new List<string>()
                    {
                    "Response: Bad Request",
                    $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                    $"Message: {ex.Message}"
                    }
                };
            }
        }

        public async Task<HttpStatusResult> DeleteStock(int id)
        {
            try
            {
                var status = await _stockRepo.DeleteStockFromRepo(id);
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = status.ToStockDTO()
                };
            }
            catch(Exception ex)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = new List<string>()
                    {
                    "Response: Bad Request",
                    $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                    $"Message: {ex.Message}"
                    }
                };
            }
        }

        public async Task<HttpStatusResult> EditStockInfo(int id, UpdateStockDTO updatestock)
        {
            try
            {
                var stockmodel = await _stockRepo.GetStockById(id);

                stockmodel.Symbol = updatestock.Symbol;
                stockmodel.CompanyName = updatestock.CompanyName;
                stockmodel.Purchase = updatestock.Purchase;
                stockmodel.Div = updatestock.LastDiv;
                stockmodel.Industry = updatestock.Industry;
                stockmodel.MarketCap = updatestock.MarketCap;

                _stockRepo.SaveChangedtoDb(stockmodel);


                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = stockmodel.ToStockDTO()
                };

            }
            catch (Exception ex)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = new List<string>()
                    {
                    "Response: Bad Request",
                    $"ErrorCode: {(int)HttpStatusCode.NotFound}",
                    $"Message: {ex.Message}"
                    }
                };
            }
        }

        public async Task<HttpStatusResult> GetAllStocks()
        {
            var AlLStocks = await _stockRepo.GetAllStocks();
            List<StockDTO> SingleStocks = null;
            
            if (AlLStocks.Count == 0)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = new List<string>()
                    {
                        $"Response: Not found",
                        $"ErrorCode: {(int)HttpStatusCode.NotFound}",
                        $"Message: Stocks not found"
                    }
                };
            }
            var SingleSTock = AlLStocks.Select(x=>x.ToStockDTO()).ToList();


            return new HttpStatusResult()
            {
                StatusCode = HttpStatusCode.OK,
                Message = SingleSTock
            };
        }

        public async Task<HttpStatusResult> GetstockbyId(int id)
        {
            try
            {
                var Stockkk = await _stockRepo.GetStockById(id);
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = Stockkk.ToStockDTO()
                };
            }
            catch( Exception ex )
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = new List<string>()
                    {
                    "Response: Bad Request",
                    $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                    $"Message: {ex.Message}"
                    }
                };
            }
        }

        public async Task<HttpStatusResult> SearchStock(string CompanyName)
        {
            try
            {
                if (string.IsNullOrEmpty(CompanyName))
                {
                    return new HttpStatusResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = new List<string>()
                    {
                        "Response: Not Found",
                        $"ErrorCode: {(int)HttpStatusCode.NotFound}",
                        "Message: Stock not found"
                    }
                    };
                }
                var StockFound = await _stockRepo.SearchCompanybyName(CompanyName);
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = new List<string>()
                    {
                        $"Company Name: {StockFound.CompanyName.ToUpper()}",
                        $"Industry: {StockFound.Industry}",
                        $"Market Cap: {StockFound.MarketCap}"
                    }
                };
            }
            catch( Exception ex )
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = new List<string>()
                    {
                        "Response: Not Found",
                        $"ErrorCode: {(int)HttpStatusCode.NotFound}",
                        "Message: Stock not found"
                    }
                };
            }
        }
    }
}
