using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyStore.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        public string? Name { get; set; }

        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
