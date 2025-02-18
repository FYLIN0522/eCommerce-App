using MyStore.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MyStore.Dtos.ProductDtos;

namespace MyStore.Dtos.OrderDtos
{
    public class ResponseCartItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public ProductInCartorOrderDto? Product { get; set; }
    }
}
