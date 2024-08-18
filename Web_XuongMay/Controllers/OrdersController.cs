using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_XuongMay.Data;
using Web_XuongMay.Models;
using Microsoft.EntityFrameworkCore;

namespace Web_XuongMay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly MyDbContext _context;

        public OrdersController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Nạp dữ liệu liên quan (OrderProducts) khi lấy danh sách đơn hàng
            var listOrder = _context.Orders.Include(o => o.OrderProducts).ToList();
            return Ok(listOrder);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrderByID(Guid id, [FromBody] OrderModel orderModel)
        {
            if (orderModel == null || id == Guid.Empty)
            {
                return BadRequest("Invalid order data or ID.");
            }

            // Nạp dữ liệu liên quan (OrderProducts) khi cập nhật đơn hàng
            var order = _context.Orders.Include(o => o.OrderProducts).SingleOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            try
            {
                // Cập nhật thông tin đơn hàng
                order.OrderNumber = orderModel.OrderNumber;
                order.OrderDate = orderModel.OrderDate;
                order.TotalAmount = orderModel.TotalAmount;
                order.OrderProducts = orderModel.OrderProducts;

                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update order. Error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult CreateNew([FromBody] OrderModel orderModel)
        {
            if (orderModel == null)
            {
                return BadRequest("Order data is null.");
            }

            try
            {
                var order = new Order
                {
                    OrderNumber = orderModel.OrderNumber,
                    OrderDate = orderModel.OrderDate,
                    TotalAmount = orderModel.TotalAmount,
                    OrderProducts = orderModel.OrderProducts.Select(op => new OrderProduct
                    {
                        ProductId = op.ProductId,
                        Quantity = op.Quantity
                    }).ToList()
                };

                _context.Add(order);
                _context.SaveChanges();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create order. Error: {ex.Message}");
            }
        }
    }
}
