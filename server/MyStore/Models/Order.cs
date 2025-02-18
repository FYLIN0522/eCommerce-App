using MyStore.Dtos.OrderDtos;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MyStore.Common;

namespace MyStore.Models
{
    //public enum Status
    //{
    //    Pending,
    //    Paid,
    //    AwaitingShipment,
    //    Shipped,
    //    Delivered,
    //    Canceled,
    //    Declined,
    //}


    public class Order
    {
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string? ReceiverName { get; set; }

        [Required]
        public AddressDto? ReceiverAddress { get; set; }

        [Required]
        public string? ReceiverPhone { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public Status Status { get; set; } //"Pending", "Paid",

        [JsonIgnore]
        public User? User { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
