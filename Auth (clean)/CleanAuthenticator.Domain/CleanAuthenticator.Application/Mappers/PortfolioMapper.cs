using CleanAuthenticator.Application.DTOs.Portfolio;
using CleanAuthenticator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.Mappers
{
    public static class PortfolioMapper
    {
        public static PortfolioDTO ToPortfolioDTO(this Portfolio portFolio)
        {
            return new PortfolioDTO()
            {
                AccountId = portFolio.AccountId,
                StockId = portFolio.StockId,
            };
        }
    }
}
