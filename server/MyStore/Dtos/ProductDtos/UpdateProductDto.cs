using MyStore.Models;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Dtos.ProductDtos
{
    public class UpdateProductDto
    {
        [MaxLength(50)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(0, 9999999, ErrorMessage = "Price cannot be less than 0.")]
        public decimal? Price { get; set; }

        [Range(0, 9999999, ErrorMessage = "StockQuantity cannot be less than 0.")]
        public int? StockQuantity { get; set; }

        [Range(0, 9999999, ErrorMessage = "CategoryId cannot be less than 0.")]
        public int? CategoryId { get; set; }
    }
}
