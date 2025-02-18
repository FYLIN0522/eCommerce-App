using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyStore.Models
{
    public class ShoppingCart
    {
        public int ShoppingCartId { get; set; }

        [Required]
        public int UserId { get; set; }

        public ICollection<ShoppingCartItem> CartItems { get; set; } = new List<ShoppingCartItem>();
    }
}
