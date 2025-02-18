using Repositories.Concrete;
using MyStore.Models;
using MyStore.Dtos.ProductDtos;

namespace Repositories.Abstract
{
    public interface IProductRepository
    {
        Task<IEnumerable<ResponseProductDto>> GetAllProducts(ProductQuery Query);
        Task<ResponseProductDto?> GetProductId(int id);
        Task AddProductAsync(Product product);

        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        //Task<bool> ProductExists(long id);
        //Task BulkAddProduct(IEnumerable<Product> products);
        Task<bool> ExistsWithNameAsync(string title);
        Task<bool> CheckCategoryAsync(int categoryId);
        Task<Product?> GetProductEntityAsync(int productId);
    }
}
