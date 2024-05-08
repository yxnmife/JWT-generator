using Authenticator.Data;
using Authenticator.DTO.Portfolio;
using Authenticator.Interfaces;
using Authenticator.Mappers;
using Authenticator.Models;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.Repository.Port
{
    public class PortfolioRepo:IPortfolioRepo
    {

        private readonly ApplicationDbContext _db;
        public PortfolioRepo(ApplicationDbContext db)
        {
            _db = db;
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

    }
}
