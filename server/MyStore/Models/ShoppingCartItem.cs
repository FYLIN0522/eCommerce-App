using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyStore.Models
{
    public class ShoppingCartItem
    {
        public int ShoppingCartItemId { get; set; }

        [Required]
        public int ShoppingCartId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [JsonIgnore]
        public ShoppingCart? ShoppingCart { get; set; }
        public Product? Product { get; set; }
    }
}
