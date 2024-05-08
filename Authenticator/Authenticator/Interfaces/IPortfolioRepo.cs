using Authenticator.DTO.Portfolio;
using Authenticator.Models;

namespace Authenticator.Interfaces
{
    public interface IPortfolioRepo
    {
        Task<Portfolio> CreatePortfolio(int StockId);
        Task<Portfolio> GetPortfolio(int StockId);
        Task<List<PortfolioDTO>> GetAllPortfolio(int AccountId);
    }
}
