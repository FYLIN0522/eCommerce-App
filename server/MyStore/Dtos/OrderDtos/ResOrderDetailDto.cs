using MyStore.Common;
using MyStore.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyStore.Dtos.OrderDtos
{
    public class ResOrderDetailDto
    {
        [Required]
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

        public ICollection<ResponseOrderItemDto> OrderItems { get; set; } = new List<ResponseOrderItemDto>();
    }
}
