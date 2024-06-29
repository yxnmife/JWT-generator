using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.Interfaces.Port
{
    public interface IPortfolioService
    {
        Task<HttpStatusResult>CreatePortfolio(string ID,int stockId);
        Task<HttpStatusResult> GetAllPortfolio(string Id);
        Task<HttpStatusResult> RemoveStockfromPortfolio(string ID,int stockId);
    }
}
