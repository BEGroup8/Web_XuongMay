using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_XuongMay.Data;
using Web_XuongMay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Web_XuongMay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderProductsController : ControllerBase
    {
        private readonly MyDbContext _context;

        public OrderProductsController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Tổng số lượng OrderProducts
                var totalRecords = _context.OrderProducts.Count();

                // Lấy danh sách OrderProducts với phân trang
                var orderproducts = _context.OrderProducts
                    .Include(op => op.Product) // Đảm bảo rằng các thuộc tính điều hướng được bao gồm
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Tạo object chứa dữ liệu phân trang
                var paginationResult = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                    Data = orderproducts
                };

                return Ok(paginationResult);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi lấy danh sách OrderProducts. Lỗi: {ex.Message}");
            }
        }

        [HttpGet("{orderId}")]
        public IActionResult GetById(Guid orderId)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound($"Đơn hàng với ID {orderId} không tìm thấy.");
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderModel orderModel)
        {
            if (orderModel == null)
            {
                return BadRequest("Dữ liệu đơn hàng bị null.");
            }

            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                OrderNumber = orderModel.OrderNumber,
                OrderDate = orderModel.OrderDate,
                TotalAmount = orderModel.TotalAmount
            };

            _context.Orders.Add(order);

            foreach (var orderProductModel in orderModel.OrderProducts)
            {
                var orderProduct = new OrderProduct
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    ProductId = orderProductModel.ProductId,
                    Quantity = orderProductModel.Quantity
                };
                _context.OrderProducts.Add(orderProduct);
            }

            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { orderId = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                return BadRequest($"Không thể tạo đơn hàng. Lỗi: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrderProduct(Guid id, [FromBody] OrderProduct updatedOrderProduct)
        {
            if (updatedOrderProduct == null || id == Guid.Empty)
            {
                return BadRequest("Invalid order product data or ID.");
            }

            var existingOrderProduct = _context.OrderProducts.SingleOrDefault(op => op.Id == id);
            if (existingOrderProduct == null)
            {
                return NotFound($"OrderProduct with ID {id} not found.");
            }

            try
            {
                existingOrderProduct.Quantity = updatedOrderProduct.Quantity;
                _context.OrderProducts.Update(existingOrderProduct);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update order product. Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrderProduct(Guid id)
        {
            var orderProduct = _context.OrderProducts.SingleOrDefault(op => op.Id == id);
            if (orderProduct == null)
            {
                return NotFound($"OrderProduct with ID {id} not found.");
            }

            try
            {
                _context.OrderProducts.Remove(orderProduct);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete order product. Error: {ex.Message}");
            }
        }
    }
}
