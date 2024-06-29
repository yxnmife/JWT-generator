using CleanAuthenticator.Application.DTOs.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.Interfaces.Stock
{
    public interface IStockService
    {
        Task<HttpStatusResult> CreateStock(CreateStockDTO create);
        Task<HttpStatusResult> GetstockbyId(int id);
        Task<HttpStatusResult> GetAllStocks();
        Task<HttpStatusResult> SearchStock(string CompanyName);
        Task<HttpStatusResult> EditStockInfo(int id, UpdateStockDTO update);
        Task<HttpStatusResult> DeleteStock(int id);
    }
}
