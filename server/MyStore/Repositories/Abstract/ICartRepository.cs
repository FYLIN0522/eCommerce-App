using MyStore.Dtos;
using MyStore.Dtos.OrderDtos;
using MyStore.Models;

namespace MyStore.Repositories.Abstract
{
    public interface ICartRepository
    {
        Task CreateCart(int userId);

        Task<ShoppingCart?> GetCartEntity(int userId);
        Task<ResponseCartDto?> GetCartDetails(int userId);
        Task AddCartItem(ShoppingCart userCart, int productId, int quantity);
        Task RemoveCartItem(ShoppingCart userCart, int productId, int quantity);
        Task<Product?> ProductExist(int productId);
    }
}
