using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyStore.Data;
using MyStore.Dtos.OrderDtos;
using MyStore.Dtos.UserDtos;
using MyStore.Exceptions;
using MyStore.Models;
using MyStore.Repositories.Abstract;
using MyStore.Service;

namespace MyStore.Controllers
{
    [ApiController]
    [Route("Users")]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly JWTService _JWTservice;

        public OrderController(OrderService orderService, JWTService service)
        {
            _orderService = orderService;
            _JWTservice = service;
        }

        // GET: api/<UserId>/Order/
        [HttpGet("{userId}/Order")]
        [Authorize]
        public async Task<IActionResult> GetOrderList(int userId)
        {
            var user = HttpContext.User;
            JWTUserInfoDto userInfo = _JWTservice.GetJWTUserInfo(user);

            if (int.Parse(userInfo.UserId) != userId)
            {
                return StatusCode(403, "Can not view another user's orders");
            }

            var cart = await _orderService.GetOrderForUser(userId);
            if (cart == null)
            {
                return NotFound("Not Found. No order with specified User ID, or user has no order");
            }

            return Ok(cart);
        }

        // GET: api/<UserId>/Order/
        [HttpGet("{userId}/Order/{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetOrder(int userId, int orderId)
        {
            var user = HttpContext.User;
            JWTUserInfoDto userInfo = _JWTservice.GetJWTUserInfo(user);

            if (int.Parse(userInfo.UserId) != userId)
            {
                return StatusCode(403, "Can not view another user's orders");
            }

            var cart = await _orderService.GetOrder(orderId);
            if (cart == null)
            {
                return NotFound("Not Found. No order with specified Order ID");
            }

            return Ok(cart);
        }

        // POST: api/<UserId>/Order
        [HttpPost("{userId}/Order")]
        [Authorize]
        public async Task<ActionResult<Order>> CreateOrder(int userId, CreateOrderDto details)
        {
            var user = HttpContext.User;
            JWTUserInfoDto userInfo = _JWTservice.GetJWTUserInfo(user);

            if (int.Parse(userInfo.UserId) != userId)
            {
                return StatusCode(403, "Can not view another user's order list");
            }

            try
            {
                var userOrder = await _orderService.CreateOrder(userId, details);

                return StatusCode(201);
            }
            catch (EmptyException err)
            {
                return NotFound(err.Message);
            }
            catch (OutOfStockException err)
            {
                return StatusCode(403, err.Message);
            }
            catch (NotFoundException err)
            {
                return NotFound(err.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        //PATCH: 
        [HttpPatch("{userId}/Order/{orderId}/Pay")]
        [Authorize]
        public async Task<ActionResult<Order>> PayOrder(int userId, int orderId, PayOrderDto details)
        {
            var user = HttpContext.User;
            JWTUserInfoDto userInfo = _JWTservice.GetJWTUserInfo(user);

            if (int.Parse(userInfo.UserId) != userId)
            {
                return StatusCode(403, "Can not view another user's order list");
            }

            try
            {
                var userOrder = await _orderService.PayOrder(orderId, details);

                return Ok("Completed");
            }
            catch (EmptyException err)
            {
                return NotFound(err.Message);
            }
            catch (NotFoundException err)
            {
                return NotFound(err.Message);
            }
            catch (OutOfStockException err)
            {
                return StatusCode(403, err.Message);
            }
            catch (StatusException err)
            {
                return StatusCode(403, err.Message);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
           
        }


        //PATCH:
        [HttpPatch("{userId}/Order/{orderId}/Cancel")]
        [Authorize]
        public async Task<ActionResult> CancelOrder(int userId, int orderId)
        {
            var user = HttpContext.User;
            JWTUserInfoDto userInfo = _JWTservice.GetJWTUserInfo(user);

            if (int.Parse(userInfo.UserId) != userId)
            {
                return StatusCode(403, "Can not view another user's order list");
            }
            try
            {
                await _orderService.CancelOrder(userId, orderId);
                return Ok("Completed");
            }
            catch (StatusException err)
            {
                return StatusCode(403, err.Message);
            }
            catch (NotFoundException)
            {
                return NotFound("The order does not exist");
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

        }
    }
}
