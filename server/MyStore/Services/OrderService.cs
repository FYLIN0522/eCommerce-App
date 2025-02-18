using Microsoft.EntityFrameworkCore;
using MyStore.Data;
using MyStore.Dtos.OrderDtos;
using MyStore.Exceptions;
using MyStore.Models;
using MyStore.Repositories.Abstract;
using MyStore.Common;
using System.Linq;
using MyStore.Dtos.ProductDtos;

namespace MyStore.Service
{
    public class OrderService
    {
        private readonly StoreContext _context;
        private readonly ICartRepository _repository;

        public OrderService(StoreContext context, ICartRepository repository)
        {
            _context = context;
            _repository = repository;
        }

//{
//  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ1c2VyQGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiVXNlciIsImV4cCI6MTczOTQzODQ5NCwiaXNzIjoiQmx1ZXRvYWQ6IE15U3RvcmUifQ.gpK5BYGCMvGL-qlBKj_dsuCKfHxXFzaw8yhhzFPeayE",
//  "refreshToken": "1JkdDvLaee+OqOZX7oVGVhBpcXZAzlIQ/1HKzTkKL4A="
//}
        public async Task<Order> CreateOrder(int userId, CreateOrderDto details)
        {
            var cart = await _repository.GetCartEntity(userId);
            if (cart is null || cart.CartItems.Count == 0)
            {
                throw new EmptyException("Cart empty");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal totalAmount = 0m;

                var order = new Order
                            {
                                UserId = userId,
                                ReceiverName = details.ReceiverName,
                                ReceiverPhone = details.ReceiverPhone,
                                ReceiverAddress = details.ReceiverAddress,

                                CreatedAt = DateTime.UtcNow,
                                Status = Status.Pending
                            };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();


                foreach (var item in cart.CartItems)
                {
                    //var product = await GetProduct(item.ProductId);
                    var product = item.Product;
                    if (product is null)
                    {
                        throw new NotFoundException("Product does not exist");
                    }
                    if (product.StockQuantity < item.Quantity)
                    {
                        throw new OutOfStockException("Product out of stock");
                    }
                    //product.StockQuantity -= item.Quantity;

                    var subTotal = product.Price * item.Quantity;
                    totalAmount += subTotal;

                    var orderItem = new OrderItem
                                    {
                                        OrderId = order.OrderId,
                                        ProductId = product.ProductId,
                                        UnitPrice = product.Price,
                                        Quantity = item.Quantity,
                                        SubTotal = subTotal
                                    };
                    _context.OrderItems.Add(orderItem);
                }
                await _context.SaveChangesAsync();

                order.TotalAmount = totalAmount;
                _context.ShoppingCartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task<Order> PayOrder(int orderId, PayOrderDto details)
        {
            try
            {
                var userOrder = await GetOrderTrack(orderId);
                if (userOrder is null)
                {
                    throw new NotFoundException("The order does not exist");
                }
                if (userOrder.OrderItems is null || userOrder.OrderItems.Count == 0)
                {
                    throw new NotFoundException("The order not include any items");
                }
                if (userOrder.Status != Status.Pending)
                {
                    throw new StatusException("Your order is paid or canceled");
                }

                foreach (var item in userOrder.OrderItems)
                {
                    var product = item.Product;
                    if (product is null)
                    {
                        throw new NotFoundException("One or more items in your order is not found");
                    }
                    if (product.StockQuantity < item.Quantity)
                    {
                        throw new OutOfStockException("Product is out of stock");
                    }
                    product.StockQuantity -= item.Quantity;
                }

                // Paid
                userOrder.Status = Status.Paid;
                await _context.SaveChangesAsync();

                return userOrder;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task CancelOrder(int userId, int orderId)
        {
            var userOrder = await GetOrderTrack(orderId);
            if (userOrder is null)
            {
                throw new NotFoundException("");
            }

            if (userOrder.Status != Status.Pending)
            {
                throw new StatusException("Your order can not be cancel");
            }

            userOrder.Status = Status.Canceled;
            await _context.SaveChangesAsync();
        }


        public async Task<Order?> GetOrderTrack(int orderID)
        {
            return await _context.Orders
                .Include(oi => oi.OrderItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Category)
                .SingleOrDefaultAsync(o => o.OrderId == orderID);
        }

        public async Task<ResOrderDetailDto?> GetOrder(int orderID)
        {
            return await _context.Orders
                .Include(oi => oi.OrderItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Category)
                .AsNoTracking()
                .Select(o => new ResOrderDetailDto
                {
                    UserId = o.UserId,
                    OrderId = o.OrderId,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    ReceiverName = o.ReceiverName,
                    ReceiverAddress = o.ReceiverAddress,
                    ReceiverPhone = o.ReceiverPhone,
                    OrderItems = o.OrderItems
                    .Select(oi => new ResponseOrderItemDto
                    {
                        //OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        Product = new ProductInCartorOrderDto
                        {
                            Name = oi.Product.Name,
                            Price = oi.Product.Price,
                        }
                    }).ToList()
                })
                .SingleOrDefaultAsync(o => o.OrderId == orderID);
        }

        public async Task<List<ResponseOrderDto>?> GetOrderForUser(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .AsNoTracking()
                .Select(o => new ResponseOrderDto
                {
                    UserId = o.UserId,
                    OrderId = o.OrderId,
                    TotalAmount = o.TotalAmount,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    OrderItems = o.OrderItems
                        .Select(oi => new ResponseOrderItemDto
                        {
                            //OrderItemId = oi.OrderItemId,
                            ProductId = oi.ProductId,
                            Quantity = oi.Quantity,
                            Product = new ProductInCartorOrderDto
                            {
                                Name = oi.Product.Name,
                                Price = oi.Product.Price,
                            }
                        }).ToList()
                }).ToListAsync();
        }

        //public async Task<Product?> GetProduct(int productId)
        //{
        //    return await _context.Products
        //        //.AsNoTracking()
        //        .SingleOrDefaultAsync(p => p.ProductId == productId);
        //}
    }
}
