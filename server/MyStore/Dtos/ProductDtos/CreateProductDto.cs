using MyStore.Models;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Dtos.ProductDtos
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        [Range(0, 9999999, ErrorMessage = "Price cannot be less than 0.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 9999999, ErrorMessage = "Stock quantity cannot be less than 0.")]
        public int StockQuantity { get; set; }

        [Required]
        [Range(0, 9999999, ErrorMessage = "Category Id cannot be less than 0.")]
        public int CategoryId { get; set; }
    }
}
