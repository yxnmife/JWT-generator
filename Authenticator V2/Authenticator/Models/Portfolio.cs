using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Authenticator.Models
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
