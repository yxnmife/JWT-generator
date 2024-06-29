using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Domain
{
   public class Stocks
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // This attribute ensures auto-increment
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
