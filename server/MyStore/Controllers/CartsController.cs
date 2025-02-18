using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyStore.Dtos.UserDtos;
using MyStore.Models;
using MyStore.Repositories.Abstract;
using MyStore.Service;
using Repositories.Abstract;
using System.Security.Claims;

namespace MyStore.Controllers
{
    [ApiController]
    [Route("Users")]
    public class CartsController : Controller
    {
        private readonly ICartRepository _repository;
        private readonly JWTService _service;
        public CartsController(ICartRepository repository, JWTService service)
        {
            _repository = repository;
            _service = service;
        }

        // GET: api/ShoppingCarts/<UserId>
        [HttpGet("{userId}/ShoppingCart")]
        [Authorize]
        public async Task<IActionResult> GetCart(int userId)
        {
            var user = HttpContext.User;
            JWTUserInfoDto userInfo = _service.GetJWTUserInfo(user);

            if (int.Parse(userInfo.UserId) != userId)
            {
                return StatusCode(403, "Can not view another user's shopping cart");
            }

            var cart = await _repository.GetCartDetails(userId);
            if (cart == null)
            {
                await _repository.CreateCart(userId);
                return Ok(cart);
            } 

            return Ok(cart);
        }

        // POST: api/ShoppingCarts/<userId>
        [HttpPost("{userId}/ShoppingCart")]
        [Authorize]
        public async Task<IActionResult> AddToCart(int userId, int productId, int quantity)
        {
            var user = HttpContext.User;
            JWTUserInfoDto userInfo = _service.GetJWTUserInfo(user);

            if (int.Parse(userInfo.UserId) != userId)
            {
                return StatusCode(403, "Can not edit another user's shopping cart");
            }
                    
            var userCart = await _repository.GetCartEntity(userId);
            if (userCart is null)
            {
                await _repository.CreateCart(userId);
            }

            var product = await _repository.ProductExist(productId);
            if (product is null)
            {
                return NotFound("Product does not exist");
            }
            if (product.StockQuantity <= 0)
            {
                return StatusCode(403, "Product out of stock");
            }

            var item = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (item is not null && (product.StockQuantity < (quantity + item.Quantity)))
            {  
                return StatusCode(403, "Not enough items");
            }


            await _repository.AddCartItem(userCart, productId, quantity);
            return StatusCode(201, "Product added To Shopping Cart");
            //return CreatedAtAction(nameof(GetCart), new { id = newProductReq.ProductId }, newProductReq);
        }

        // POST: api/Carts
        [HttpDelete("{userId}/ShoppingCart")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int userId, int productId, int quantity)
        {
            var user = HttpContext.User;
            JWTUserInfoDto userInfo = _service.GetJWTUserInfo(user);

            if (int.Parse(userInfo.UserId) != userId)
            {
                return StatusCode(403, "Can not edit another user's shopping cart");
            }

            var userCart = await _repository.GetCartEntity(userId);
            if (userCart is null)
            {
                return NotFound("Your shopping cart is empty");
            }

            var productInCart = userCart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (productInCart is null)
            {
                return NotFound("Product does not exist in shopping cart");
            }

            await _repository.RemoveCartItem(userCart, productId, quantity);
            return Ok("Item remove from your shopping cart");
            //return CreatedAtAction(nameof(GetCart), new { id = newProductReq.ProductId }, newProductReq);
        }
    }
}
