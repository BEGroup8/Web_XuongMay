using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_XuongMay.Data;
using Web_XuongMay.Models;
using System;
using System.Linq;
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

        // GET: api/Orders
        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Tổng số lượng Orders
                var totalRecords = _context.Orders.Count();

                // Lấy danh sách Orders với phân trang
                var orders = _context.Orders
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
                    Data = orders
                };

                return Ok(paginationResult); // Trả về kết quả với HTTP 200 OK
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // GET: api/Orders/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok(order);
        }

        // POST: api/Orders
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
                    OrderId = Guid.NewGuid(),
                    OrderNumber = orderModel.OrderNumber,
                    OrderDate = orderModel.OrderDate,
                    TotalAmount = orderModel.TotalAmount
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                return Ok(new
                {
                    Success = true,
                    Data = order
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create order. Error: {ex.Message}");
            }
        }

        // PUT: api/Orders/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOrderByID(Guid id, [FromBody] OrderModel orderModel)
        {
            if (orderModel == null || id == Guid.Empty)
            {
                return BadRequest("Invalid order data or ID.");
            }

            var order = _context.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            try
            {
                order.OrderNumber = orderModel.OrderNumber;
                order.OrderDate = orderModel.OrderDate;
                order.TotalAmount = orderModel.TotalAmount;

                _context.Orders.Update(order);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update order. Error: {ex.Message}");
            }
        }

        // DELETE: api/Orders/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(Guid id)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            try
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete order. Error: {ex.Message}");
            }
        }
    }
}
