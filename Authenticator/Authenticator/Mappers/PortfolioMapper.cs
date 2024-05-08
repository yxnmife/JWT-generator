using Authenticator.DTO.Portfolio;
using Authenticator.Models;

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
