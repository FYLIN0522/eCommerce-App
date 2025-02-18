using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyStore.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        [Range(0, 9999999, ErrorMessage = "Price cannot be less than 0.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 9999999, ErrorMessage = "StockQuantity cannot be less than 0.")]
        public int StockQuantity { get; set; }

        [JsonIgnore]
        public string? ImageUrl { get; set; }

        [Required]
        [Range(0, 9999999, ErrorMessage = "CategoryId cannot be less than 0.")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}