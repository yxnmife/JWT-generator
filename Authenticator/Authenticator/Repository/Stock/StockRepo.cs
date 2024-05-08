using Authenticator.Data;
using Authenticator.DTO.Stock;
using Authenticator.Interfaces;
using Authenticator.Mappers;
using Authenticator.Models;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.Repository.Stock
{
    public class StockRepo:IStockRepo
    {
        private readonly ApplicationDbContext _db;
        public StockRepo(ApplicationDbContext db)
        {
            _db= db;
        }
        public async Task<Stocks> CreateStocks(CreateStockDTO createstock, Stocks stockmodel)
        {
            var isExisting = await _db.StockTable.AnyAsync(x=>x.CompanyName==createstock.CompanyName);
            if(isExisting)
            {
                return null;
            }
            var isExisting2 = await _db.StockTable.AnyAsync(x => x.Symbol == createstock.Symbol);
            if (isExisting2)
            {
                return null;
            }


            stockmodel.Symbol = createstock.Symbol;
            stockmodel.CompanyName = createstock.CompanyName;
            stockmodel.Purchase = createstock.Purchase;
            stockmodel.Div = createstock.LastDiv;
            stockmodel.Industry = createstock.Industry;
            stockmodel.MarketCap = createstock.MarketCap;
            await _db.StockTable.AddAsync(stockmodel);
            await _db.SaveChangesAsync();
            return stockmodel;

        }

        public async Task<Stocks> EditStock(UpdateStockDTO updatestock)
        {
            var stockmodel = new Stocks();

            stockmodel.Symbol = updatestock.Symbol;
            stockmodel.CompanyName = updatestock.CompanyName;
            stockmodel.Purchase = updatestock.Purchase;
            stockmodel.Div = updatestock.LastDiv;
            stockmodel.Industry = updatestock.Industry;
            stockmodel.MarketCap = updatestock.MarketCap;

            await _db.SaveChangesAsync();
            return stockmodel;
        }
        
        public async Task<List<StockDTO>> GetAll()
        {
            var items = await _db.StockTable.Select(x=>x.ToStockDTO()).ToListAsync();
            return items;
        }
        public async Task<Stocks>GetStockbyID(int id)
        {
            var stock = await _db.StockTable.FirstOrDefaultAsync(x => x.Id == id);
            if (stock == null)
            {
                return null;
            }

            return stock;
        }
        public async Task<Stocks>GetByName(string name)
        {
            var stock = await _db.StockTable.FirstOrDefaultAsync(x => x.CompanyName.Contains(name));
            if (stock == null)
            {
                return null;
            }

            return stock;
        }
    }

}
