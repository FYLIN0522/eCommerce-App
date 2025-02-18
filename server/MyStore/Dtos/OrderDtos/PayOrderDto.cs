using System.ComponentModel.DataAnnotations;

namespace MyStore.Dtos.OrderDtos
{
    public class PayOrderDto
    {
        [Required]
        [RegularExpression(@"^\d{16}$")]
        public string? CardNum { get; set; }

        [Required]
        [RegularExpression(@"^\d{2}/\d{2}$")]
        public string? ExpirationDate { get; set; }

        [Required]
        public string? CardHolder { get; set; }

        [Required]
        [RegularExpression(@"^\d{3}$")]
        public string? CVV { get; set; }
    }
}
