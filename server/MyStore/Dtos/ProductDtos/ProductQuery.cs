using System.ComponentModel.DataAnnotations;

namespace MyStore.Dtos.ProductDtos
{
    public class ProductQuery
    {
        public string? Q { get; set; }
        public int? CategoryIds { get; set; } = 0;

        [Range(0, 9999999, ErrorMessage = "minPrice cannot be less than 0.")]
        public int? minPrice { get; set; } = 0;

        [Range(0, 9999999, ErrorMessage = "maxPrice cannot be less than 0.")]
        public int? maxPrice { get; set; }

        public string? SortBy { get; set; }
        public int Count { get; set; } = 10;
    }
}
