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
            // Tìm đơn hàng theo OrderId
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                // Nếu không tìm thấy đơn hàng, trả về HTTP 404 Not Found với thông báo lỗi
                return NotFound($"Đơn hàng với ID {orderId} không tìm thấy.");
            }

            // Trả về đối tượng đơn hàng nếu tìm thấy với HTTP 200 OK
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderModel orderModel)
        {
            if (orderModel == null)
            {
                // Nếu dữ liệu đầu vào bị null, trả về HTTP 400 Bad Request với thông báo lỗi
                return BadRequest("Dữ liệu đơn hàng bị null.");
            }

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

            // Duyệt qua danh sách sản phẩm trong đơn hàng và thêm vào DbContext
            foreach (var orderProductModel in orderModel.OrderProducts)
            {
                var orderProduct = new OrderProduct
                {
                    Id = Guid.NewGuid(), // Tạo một ID mới cho OrderProduct
                    OrderId = order.OrderId,
                    ProductId = orderProductModel.ProductId,
                    Quantity = orderProductModel.Quantity
                };
                _context.OrderProducts.Add(orderProduct);
            }

            try
            {
                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
                // Trả về HTTP 201 Created với thông tin về đơn hàng mới được tạo
                return CreatedAtAction(nameof(GetById), new { orderId = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                // Nếu xảy ra lỗi khi lưu dữ liệu, trả về HTTP 400 Bad Request với thông báo lỗi
                return BadRequest($"Không thể tạo đơn hàng. Lỗi: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrderProduct(Guid id, [FromBody] OrderProduct updatedOrderProduct)
        {
            if (updatedOrderProduct == null || id == Guid.Empty)
            {
                // Nếu dữ liệu đầu vào bị null hoặc ID không hợp lệ, trả về HTTP 400 Bad Request
                return BadRequest("Invalid order product data or ID.");
            }

            // Tìm đối tượng OrderProduct theo ID
            var existingOrderProduct = _context.OrderProducts.SingleOrDefault(op => op.Id == id);
            if (existingOrderProduct == null)
            {
                // Nếu không tìm thấy đối tượng, trả về HTTP 404 Not Found
                return NotFound($"OrderProduct with ID {id} not found.");
            }

            try
            {
                // Cập nhật số lượng sản phẩm trong đơn hàng
                existingOrderProduct.Quantity = updatedOrderProduct.Quantity;
                // Cập nhật đối tượng trong DbContext
                _context.OrderProducts.Update(existingOrderProduct);
                // Lưu các thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
                // Trả về HTTP 204 No Content nếu cập nhật thành công
                return NoContent();
            }
            catch (Exception ex)
            {
                // Nếu xảy ra lỗi khi lưu dữ liệu, trả về HTTP 400 Bad Request với thông báo lỗi
                return BadRequest($"Failed to update order product. Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrderProduct(Guid id)
        {
            // Tìm đối tượng OrderProduct theo ID
            var orderProduct = _context.OrderProducts.SingleOrDefault(op => op.Id == id);
            if (orderProduct == null)
            {
                // Nếu không tìm thấy đối tượng, trả về HTTP 404 Not Found
                return NotFound($"OrderProduct with ID {id} not found.");
            }

            try
            {
                // Xóa đối tượng OrderProduct khỏi DbContext
                _context.OrderProducts.Remove(orderProduct);
                // Lưu các thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
                // Trả về HTTP 204 No Content nếu xóa thành công
                return NoContent();
            }
            catch (Exception ex)
            {
                // Nếu xảy ra lỗi khi lưu dữ liệu, trả về HTTP 400 Bad Request với thông báo lỗi
                return BadRequest($"Failed to delete order product. Error: {ex.Message}");
            }
        }

    }
}
