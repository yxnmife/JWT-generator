using Authenticator.DTO.Stock;
using Authenticator.Extensions;
using Authenticator.Models;

namespace Authenticator.Interfaces
{
    public interface IStockRepo
    {
        Task<Stocks> CreateStocks(CreateStockDTO createstock);
        public HttpStatusResult StockExistsResponse();
        Task<HttpStatusResult> EditStockbyIDValidation(int id, UpdateStockDTO update);
        Task<HttpStatusResult> StockDeleteValidation(Stocks stocks);
        Task<HttpStatusResult> GetbyIdfromCache(int id);
        Task<HttpStatusResult> GetAllStocksValidation();
        Task<HttpStatusResult> EditStockbyNameValidation(UpdateStockDTO update, string name);
    }
}
