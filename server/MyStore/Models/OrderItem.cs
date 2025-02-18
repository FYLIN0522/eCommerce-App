using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyStore.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal SubTotal { get; set; }  // UnitPrice * Quantity

        [JsonIgnore]
        public Order? Order { get; set; }

        public Product? Product { get; set; }
    }
}
