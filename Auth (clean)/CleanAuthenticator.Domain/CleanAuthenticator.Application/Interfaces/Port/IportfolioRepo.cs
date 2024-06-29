using CleanAuthenticator.Application.DTOs.Portfolio;
using CleanAuthenticator.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.Interfaces.Port
{
    public interface IportfolioRepo
    {
        Task<Portfolio> CREATE(Portfolio portfolio);
        List<PortfolioDTO> GetAll(int accountId);
        Task<Portfolio> FindbyStockId(int stockId);
        Task<Portfolio>DeletePortfolio(Portfolio portfolio);
    }
}
