using MyStore.Models;
using System.ComponentModel.DataAnnotations;

namespace MyStore.Dtos.OrderDtos
{
    public class ResponseCartDto
    {
        [Required]
        public int ShoppingCartId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public ICollection<ResponseCartItemDto>? CartItems { get; set; }
    }
}
