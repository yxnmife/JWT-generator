using CleanAuthenticator.Application.DTOs.Stock;
using CleanAuthenticator.Application.Interfaces.Stock;
using CleanAuthenticator.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CleanAuthenticator.Infrastructure
{
    public class StockRepo : IStockRepo
    {
        private readonly ApplicationDbContext _db;
        public StockRepo(ApplicationDbContext db)
        {
            _db= db;
        }
        public async Task<Stocks> CreateStocks(Stocks NewStock)
        {
            var isExisting = await _db.StockTable.AnyAsync(x => x.CompanyName.ToLower() == NewStock.CompanyName.ToLower() || x.Symbol.ToLower() == NewStock.Symbol.ToLower());
            if (isExisting)
            {
                throw new Exception("Stock with same CompanyName/Symbol already exists");
            };
            await _db.StockTable.AddAsync(NewStock);
            await _db.SaveChangesAsync();
            return NewStock;
        }

        public async Task<Stocks> DeleteStockFromRepo(int id)
        {
            var stocks = await _db.StockTable.FindAsync(id);
            if (stocks == null)
            {
                throw new Exception("Stock not found");
            }

            _db.StockTable.Remove(stocks);
            return stocks;
        }

        public async Task<List<Stocks>> GetAllStocks()
        {
            return await _db.StockTable.ToListAsync();
        }


        public async Task<Stocks> GetStockById(int id)
        {
            var stockk = await _db.StockTable.FindAsync(id);
            if(stockk== null)
            {
                throw new Exception("Stock not found");
            }
            return stockk;
        }

        public void SaveChangedtoDb(Stocks newStock)
        {
            _db.StockTable.Update(newStock);
            _db.SaveChanges();
        }

        public async Task<Stocks> SearchCompanybyName(string companyName)
        {
            var StockFound = await _db.StockTable.FirstOrDefaultAsync(a => a.CompanyName.Contains(companyName));
            if (StockFound == null)
            {
                throw new Exception("Stock not Found");
            }
            return StockFound;

        }
    }
}
