using Authenticator.DTO.Portfolio;
using Authenticator.Extensions;
using Authenticator.Models;

namespace Authenticator.Interfaces
{
    public interface IPortfolioRepo
    {
      
        Task<Portfolio> GetPortfolio(int StockId);
        Task<List<PortfolioDTO>> GetAllPortfolio(int AccountId);
        Task<HttpStatusResult> PortfolioCreationValidation(int StockID, string Id);
        Task<HttpStatusResult> PortfolioDeletionValidation(Portfolio portfolio);
        Task<HttpStatusResult> GetPortfoliofromCache(int AccountId);
    }
}
