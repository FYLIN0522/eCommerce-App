using MyStore.Models;
using System.ComponentModel.DataAnnotations;
using MyStore.Common;

namespace MyStore.Dtos.OrderDtos
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

    public class ResponseOrderDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        //[Required]
        //public string? ReceiverName { get; set; }

        //[Required]
        //public AddressDto? ReceiverAddress { get; set; }

        //[Required]
        //public string? ReceiverPhone { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public Status Status { get; set; } //"Pending", "Paid",

        //public User? User { get; set; }

        public ICollection<ResponseOrderItemDto> OrderItems { get; set; } = new List<ResponseOrderItemDto>();
    }
}
