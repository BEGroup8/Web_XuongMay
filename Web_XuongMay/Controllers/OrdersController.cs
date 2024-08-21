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
                // Đếm tổng số lượng đơn hàng trong cơ sở dữ liệu
                var totalRecords = _context.Orders.Count();

                // Lấy danh sách đơn hàng với phân trang
                var orders = _context.Orders
                    .Skip((pageNumber - 1) * pageSize) // Bỏ qua các đơn hàng của trang trước
                    .Take(pageSize) // Lấy số lượng đơn hàng theo kích thước trang
                    .ToList();

                // Tạo object chứa thông tin phân trang
                var paginationResult = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize), // Tính tổng số trang
                    Data = orders // Dữ liệu đơn hàng cho trang hiện tại
                };

                // Trả về dữ liệu với HTTP 200 OK
                return Ok(paginationResult);
            }
            catch (Exception ex)
            {
                // Nếu xảy ra lỗi, trả về HTTP 400 Bad Request với thông báo lỗi
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // GET: api/Orders/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // Tìm đơn hàng theo OrderId
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                // Nếu không tìm thấy đơn hàng, trả về HTTP 404 Not Found
                return NotFound($"Order with ID {id} not found.");
            }
            // Nếu tìm thấy, trả về đối tượng đơn hàng với HTTP 200 OK
            return Ok(order);
        }

        // POST: api/Orders
        [HttpPost]
        public IActionResult CreateNew([FromBody] OrderModel orderModel)
        {
            if (orderModel == null)
            {
                // Nếu dữ liệu đầu vào bị null, trả về HTTP 400 Bad Request
                return BadRequest("Order data is null.");
            }

            try
            {
                // Tạo một đối tượng Order mới từ dữ liệu của OrderModel
                var order = new Order
                {
                    OrderId = Guid.NewGuid(), // Tạo một OrderId mới
                    OrderNumber = orderModel.OrderNumber,
                    OrderDate = orderModel.OrderDate,
                    TotalAmount = orderModel.TotalAmount
                };

                // Thêm đơn hàng mới vào DbContext
                _context.Orders.Add(order);
                _context.SaveChanges();

                // Trả về HTTP 200 OK với thông tin đơn hàng mới được tạo
                return Ok(new
                {
                    Success = true,
                    Data = order
                });
            }
            catch (Exception ex)
            {
                // Nếu xảy ra lỗi khi tạo đơn hàng, trả về HTTP 400 Bad Request với thông báo lỗi
                return BadRequest($"Failed to create order. Error: {ex.Message}");
            }
        }

        // PUT: api/Orders/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOrderByID(Guid id, [FromBody] OrderModel orderModel)
        {
            if (orderModel == null || id == Guid.Empty)
            {
                // Nếu dữ liệu đầu vào bị null hoặc ID không hợp lệ, trả về HTTP 400 Bad Request
                return BadRequest("Invalid order data or ID.");
            }

            // Tìm đối tượng Order theo ID
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                // Nếu không tìm thấy đơn hàng, trả về HTTP 404 Not Found
                return NotFound($"Order with ID {id} not found.");
            }

            try
            {
                // Cập nhật thông tin đơn hàng
                order.OrderNumber = orderModel.OrderNumber;
                order.OrderDate = orderModel.OrderDate;
                order.TotalAmount = orderModel.TotalAmount;

                // Cập nhật đối tượng trong DbContext
                _context.Orders.Update(order);
                _context.SaveChanges();

                // Trả về HTTP 204 No Content nếu cập nhật thành công
                return NoContent();
            }
            catch (Exception ex)
            {
                // Nếu xảy ra lỗi khi cập nhật đơn hàng, trả về HTTP 400 Bad Request với thông báo lỗi
                return BadRequest($"Failed to update order. Error: {ex.Message}");
            }
        }

        // DELETE: api/Orders/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(Guid id)
        {
            // Tìm đối tượng Order theo ID
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                // Nếu không tìm thấy đơn hàng, trả về HTTP 404 Not Found
                return NotFound($"Order with ID {id} not found.");
            }

            try
            {
                // Xóa đơn hàng khỏi DbContext
                _context.Orders.Remove(order);
                _context.SaveChanges();

                // Trả về HTTP 204 No Content nếu xóa thành công
                return NoContent();
            }
            catch (Exception ex)
            {
                // Nếu xảy ra lỗi khi xóa đơn hàng, trả về HTTP 400 Bad Request với thông báo lỗi
                return BadRequest($"Failed to delete order. Error: {ex.Message}");
            }
        }
    }
}
