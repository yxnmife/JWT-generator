using Authenticator.DTO.Portfolio;
using Authenticator.Models;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Authenticator.Mappers
{
    public static class PortfolioMapper
    {
        public static PortfolioDTO ToPortfolioDTO(this Portfolio portFolio)
        {
            return new PortfolioDTO()
            {
                AccountId= portFolio.AccountId,
                StockId= portFolio.StockId,
            };
        }

    }
}
