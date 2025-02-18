using MyStore.Dtos.ProductDtos;
using MyStore.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyStore.Dtos.OrderDtos
{
    public class ResponseOrderItemDto
    {
        //[Required]
        //public int OrderItemId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public ProductInCartorOrderDto? Product { get; set; }
    }
}
