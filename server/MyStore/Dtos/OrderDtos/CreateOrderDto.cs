using MyStore.Models;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Dtos.OrderDtos
{
    public class CreateOrderDto
    {

        //[Required]
        //public int UserId { get; set; }

        [Required]
        public string? ReceiverName { get; set; }

        [Required]
        public AddressDto? ReceiverAddress { get; set; }

        [Required]
        public string? ReceiverPhone { get; set; }
    }
}
