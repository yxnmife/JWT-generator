using Authenticator.DTO.Stock;
using Authenticator.Models;

namespace Authenticator.Mappers
{
    public static class StockMapper
    {
        public static StockDTO ToStockDTO(this Stocks stock)
        {
            return new StockDTO()
            {
                Id = stock.Id,
                Symbol = stock.Symbol,
                CompanyName= stock.CompanyName,
                Div = stock.Div,
                Purchase = stock.Purchase,
                Industry = stock.Industry,
                MarketCap = stock.MarketCap,
               
            };
        }

    }
}
