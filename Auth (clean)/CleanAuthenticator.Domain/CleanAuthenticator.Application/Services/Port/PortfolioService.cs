using CleanAuthenticator.Application.Interfaces.Account;
using CleanAuthenticator.Application.Interfaces.Port;
using CleanAuthenticator.Application.Interfaces.Stock;
using CleanAuthenticator.Application.Mappers;
using CleanAuthenticator.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Application.Services.Port
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IportfolioRepo _repo;
        private readonly IStockRepo _stockRepo;
        private readonly IAccountRepo _accountRepo;
        public PortfolioService(IportfolioRepo repo,IStockRepo stockrepo,IAccountRepo accountRepo)
        {
            _repo= repo;
            _stockRepo= stockrepo;
            _accountRepo= accountRepo;
        }
        public async Task<HttpStatusResult> CreatePortfolio(string ID,int stockId)
        {
            try
            {
                if (stockId<=0) 
                
                {
                    throw new Exception("Invalid Stock ID");
                }

                var userId = Convert.ToInt32(ID);

                //check if account exists with ACCOUNT ID
                //exception will be thrown if account not found
                var response = await _accountRepo.GetAccountById(userId);

                //check if stock exists with StockId
                //exception will be thrown if account not found
                var result = await _stockRepo.GetStockById(stockId);

                var nEwPortfolio = new Portfolio();

                nEwPortfolio.AccountId= userId;
                nEwPortfolio.StockId = stockId;

                await _repo.CREATE(nEwPortfolio);
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = nEwPortfolio.ToPortfolioDTO()
                };

            }
            catch(DuplicateNameException ex)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = new List<string>()
                    {
                        "Response: Bad Request",
                        $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                        $"Message: {ex.Message}"
                    }
                };
            }
            catch(Exception ex)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = new List<string>()
                    {
                        "Response: Bad Request",
                        $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                        $"Message: {ex.Message}"
                    }
                };
            }
        }

        public async Task<HttpStatusResult> GetAllPortfolio(string Id)
        {
            try
            {
                var UserId = Convert.ToInt32(Id);
                var list  = _repo.GetAll(UserId);

                if (list == null)
                {
                    return new HttpStatusResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = new List<string>()
                        {
                            "Response: Not Found",
                            "ErrorCode: 404",
                            "Message: User Portfolio not found"
                        }
                    };
                }
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = list
                };
            }
            catch(Exception ex)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = new List<string>()
                    {
                        "Response: Bad Request",
                        $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                        $"Message: {ex.Message}"
                    }
                };
            }
        }

        public async Task<HttpStatusResult> RemoveStockfromPortfolio(string ID, int stockId)
        {
            try
            {
                var UserId= Convert.ToInt32(ID);
                var portFolio= await _repo.FindbyStockId(stockId);
                var response = await _repo.DeletePortfolio(portFolio);

                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = portFolio.ToPortfolioDTO()
                };
            }
            catch(Exception ex)
            {
                return new HttpStatusResult()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = new List<string>()
                    {
                        "Response: Bad Request",
                        $"ErrorCode: {(int)HttpStatusCode.BadRequest}",
                        $"Message: {ex.Message}"
                    }
                };
            }
        }
    }
}
