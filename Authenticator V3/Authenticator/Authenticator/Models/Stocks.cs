using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authenticator.Models
{
    public class Stocks
    {
      
     
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public decimal Div { get; set; }
        public decimal Purchase { get; set; }
        public string Industry { get; set; }
        public long MarketCap { get; set; }
        public List<Portfolio> Portfolios { get; set; } = new List<Portfolio>();

    }
}
