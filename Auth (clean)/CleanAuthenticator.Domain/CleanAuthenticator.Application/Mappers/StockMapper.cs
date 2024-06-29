using CleanAuthenticator.Application.DTOs.Stock;
using CleanAuthenticator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.Mappers
{
    public static class StockMapper
    {
        public static StockDTO ToStockDTO(this Stocks stock)
        {
            return new StockDTO()
            {
                Id = stock.Id,
                Symbol = stock.Symbol,
                CompanyName = stock.CompanyName,
                Div = stock.Div,
                Purchase = stock.Purchase,
                Industry = stock.Industry,
                MarketCap = stock.MarketCap,

            };
        }

    }
}
