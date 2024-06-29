using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanAuthenticator.Domain
{
    [Table("Porttfolios")]
    public class Portfolio
    {
        [Key]
        public int AccountId { get; set; }
        public int StockId { get; set; }
        public Accounts account { get; set; }
        public Stocks stocks { get; set; }
    }
}
