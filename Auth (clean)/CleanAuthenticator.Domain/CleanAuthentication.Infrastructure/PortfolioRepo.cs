using CleanAuthenticator.Application.DTOs.Portfolio;
using CleanAuthenticator.Application.Interfaces.Port;
using CleanAuthenticator.Application.Mappers;
using CleanAuthenticator.Domain;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CleanAuthenticator.Infrastructure
{
    public class PortfolioRepo : IportfolioRepo
    {
        private readonly ApplicationDbContext _db;
        public PortfolioRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Portfolio> CREATE(Portfolio portfolio)
        {
            var existing = _db.PortfolioTable.Any(a => a.StockId == portfolio.StockId);
            if (existing)
            {
                throw new DuplicateNameException("Stock already exists in Portfolio");
            }
            await _db.PortfolioTable.AddAsync(portfolio);
            await _db.SaveChangesAsync();
            return portfolio;
        }

        public async Task<Portfolio> DeletePortfolio(Portfolio portfolio)
        {
            if(portfolio == null)
            {
                throw new Exception("Stock not found in User Portfolio");
            }
            _db.PortfolioTable.Remove(portfolio);
            await _db.SaveChangesAsync();
            return portfolio;
        }

        public async Task<Portfolio> FindbyStockId(int stockId)
        {
           var port = await _db.PortfolioTable.FirstOrDefaultAsync(a=>a.StockId== stockId);
            return port;
        }

        public List<PortfolioDTO> GetAll(int accountId)
        {
            var exists = _db.PortfolioTable.Any(ab => ab.AccountId == accountId);

            if (!exists)
            {
                throw new Exception("Portfolio for user does not exist");
            }

            var portfolios = _db.PortfolioTable
                .Where(a => a.AccountId == accountId)
                .Select(a=>a.ToPortfolioDTO());
            return portfolios.ToList();
        }
    }
}
