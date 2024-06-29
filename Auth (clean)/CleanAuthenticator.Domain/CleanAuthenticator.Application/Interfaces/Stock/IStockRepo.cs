using CleanAuthenticator.Application.DTOs.Stock;
using CleanAuthenticator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.Interfaces.Stock
{
    public interface IStockRepo
    {
        Task<Stocks> CreateStocks(Stocks NewStock);
        Task<Stocks> DeleteStockFromRepo(int id);
        Task<Stocks> GetStockById(int id);
        Task<List<Stocks>> GetAllStocks();
        Task<Stocks> SearchCompanybyName(string companyName);
        void SaveChangedtoDb(Stocks newStock);

    }
}
