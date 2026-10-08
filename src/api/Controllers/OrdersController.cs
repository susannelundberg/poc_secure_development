using System.Security.Claims;
using application.DTOs.Orders;
using domain.Entities;
using infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ApiController]
    public class OrdersController(AppDbContext context) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> AddOrder(CreateOrderDTO model)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId is null) return Unauthorized();

                var orderItems = new List<OrderItem>();

                foreach (var x in model.OrderItems)
                {
                    var product = await context.Products.FirstOrDefaultAsync(s => s.Id == x.ProductId);
                    if (product is null)
                    {
                        return BadRequest(new {Success = false, message = "En av produkterna finns inte." });
                    }

                    orderItems.Add(new OrderItem
                    {
                        Product = product,
                        Quantity = x.Quantity,
                        Sum = x.Quantity * product.Price
                    });
                }                
                
                var order = new Order
                {
                    UserId = userId,
                    OrderNumber = Guid.NewGuid(),
                    OrderItems = orderItems,
                    TotalPrice = orderItems.Sum(x => x.Sum)
                };

                context.Order.Add(order);
                await context.SaveChangesAsync();

                return StatusCode(201, new { Success = true, order.OrderNumber });
            }
            catch
            {
                return StatusCode(500, "Order kunde inte skapas");
            }
        }


        [HttpGet]
        public async Task<ActionResult> ListMyOrders()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId is null) return Unauthorized();

                var orders = await context.Order
                .Where(o => o.UserId == userId)
                .Select(order => new ListOrdersDTO
                {
                    OrderNumber = order.OrderNumber,
                    TotalPrice = order.TotalPrice
                }).ToListAsync();

                return Ok(new { Success = true, Data = orders });
            }
            catch
            {
                return StatusCode(500, "Order kunde inte hämtas");
            }
        }

    }
}
