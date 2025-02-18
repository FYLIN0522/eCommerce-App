using MyStore.Models;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Dtos.ProductDtos
{
    public class ProductInCartorOrderDto
    {
        //[Required]
        //public int ProductId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        //[Required]
        //public Category? Category { get; set; }
    }
}
