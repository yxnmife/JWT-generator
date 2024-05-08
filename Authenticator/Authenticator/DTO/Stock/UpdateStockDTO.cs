using System.ComponentModel.DataAnnotations;

namespace Authenticator.DTO.Stock
{
    public class UpdateStockDTO
    {
       
        [Required]
        [MaxLength(10, ErrorMessage = "Symbol should have a maximum of 10 characters")]
        public string Symbol { get; set; }
        [Required]
        [MaxLength(20, ErrorMessage = "CompanyName should have a maximum of 20 characters")]
        public string CompanyName { get; set; }
        [Required]
        [Range(1, 1000000)]
        public decimal Purchase { get; set; }
        [Required]
        [Range(1, 1000000)]
        public decimal LastDiv { get; set; }
        [Required]
        [MaxLength(20, ErrorMessage = "Industry should have a maximum of 20 characters")]
        public string Industry { get; set; } = string.Empty;
        [Required]
        [Range(0, 5000000000000)]
        public long MarketCap { get; set; }
    }
}

