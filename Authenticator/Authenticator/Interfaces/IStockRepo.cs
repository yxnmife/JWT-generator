using Authenticator.DTO.Stock;
using Authenticator.Models;

namespace Authenticator.Interfaces
{
    public interface IStockRepo
    {
        Task<Stocks> CreateStocks(CreateStockDTO createstock, Stocks stockmodel);
        Task<Stocks> GetStockbyID(int id);
        Task<Stocks> GetByName(string name);
        Task<Stocks> EditStock(UpdateStockDTO updatestock);
        Task<List<StockDTO>> GetAll();
    }
}
