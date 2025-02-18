using Microsoft.EntityFrameworkCore;
using MyStore.Data;
using MyStore.Dtos.OrderDtos;
using MyStore.Dtos.ProductDtos;
using MyStore.Models;
using MyStore.Repositories.Abstract;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace MyStore.Repositories.Concrete
{
    public class CartRepository : ICartRepository
    {
        private readonly StoreContext _context;
        public CartRepository(StoreContext context)
        {
            _context = context;
        }
        public async Task CreateCart(int userId)
        {
            var Cart = new ShoppingCart {UserId = userId};
            _context.ShoppingCarts.Add(Cart);
            await _context.SaveChangesAsync();

            //return Cart;
        }


        public async Task<ShoppingCart?> GetCartEntity(int userId)
        {
            var Cart = await _context.ShoppingCarts
                                .Include(c => c.CartItems)
                                .ThenInclude(i => i.Product)
                                .ThenInclude(p => p.Category)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(c => c.UserId == userId);

            return Cart;
        }

        public async Task<ResponseCartDto?> GetCartDetails(int userId)
        {
            var Cart = await _context.ShoppingCarts
                                .Include(c => c.CartItems)
                                .ThenInclude(i => i.Product)
                                .ThenInclude(p => p.Category)
                                .AsNoTracking()
                                .Select(c => new ResponseCartDto
                                {
                                    ShoppingCartId = c.ShoppingCartId,
                                    UserId = c.UserId,
                                    CartItems = c.CartItems
                                        .Select(cI => new ResponseCartItemDto
                                        {
                                            ProductId = cI.ProductId,
                                            Quantity = cI.Quantity,
                                            Product = new ProductInCartorOrderDto
                                            {
                                                Name = cI.Product.Name,
                                                Price = cI.Product.Price,
                                                //Category = cI.Product.Category
                                            }
                                        }).ToList()
                                })
                                .FirstOrDefaultAsync(c => c.UserId == userId);

            return Cart;
        }


        public async Task AddCartItem(ShoppingCart userCart, int productId, int quantity)
        {
            var cartItem = userCart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem is null)
            {
                cartItem = new ShoppingCartItem
                {
                    ShoppingCartId = userCart.ShoppingCartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.ShoppingCartItems.Add(cartItem);
            } 
            else 
            {
                _context.Attach(cartItem);
                cartItem.Quantity += quantity;
                _context.Entry(cartItem).Property(i => i.Quantity).IsModified = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveCartItem(ShoppingCart userCart, int productId, int quantity)
        {
            var cartItem = userCart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem.Quantity - quantity <= 0)
            {
                _context.ShoppingCartItems.Remove(cartItem);
            }
            else
            {
                _context.Attach(cartItem);
                cartItem.Quantity -= quantity;
                _context.Entry(cartItem).Property(i => i.Quantity).IsModified = true;
            }

            await _context.SaveChangesAsync();
        }


        public async Task<Product?> ProductExist(int productId)
        {
            return await _context.Products
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.ProductId == productId);
        }
    }
}
