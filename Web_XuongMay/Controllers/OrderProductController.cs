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

        // Get all order products
        [HttpGet]
        public IActionResult GetAll()
        {
            var orderProducts = _context.OrderProducts.Include(op => op.Order)
                                                      .Include(op => op.Product)
                                                      .ToList();
            return Ok(orderProducts);
        }

        // Get a specific order product by order ID and product ID
        [HttpGet("{orderId}/{productId}")]
        public IActionResult GetById(Guid orderId, Guid productId)
        {
            var orderProduct = _context.OrderProducts.Include(op => op.Order)
                                                      .Include(op => op.Product)
                                                      .SingleOrDefault(op => op.OrderId == orderId && op.ProductId == productId);
            if (orderProduct == null)
            {
                return NotFound($"OrderProduct with OrderId {orderId} and ProductId {productId} not found.");
            }
            return Ok(orderProduct);
        }

        // Create a new order product
        [HttpPost]
        public IActionResult CreateNew([FromBody] OrderProduct orderProduct)
        {
            if (orderProduct == null)
            {
                return BadRequest("OrderProduct data is null.");
            }

            try
            {
                _context.OrderProducts.Add(orderProduct);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetById), new { orderId = orderProduct.OrderId, productId = orderProduct.ProductId }, orderProduct);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create order product. Error: {ex.Message}");
            }
        }

        // Update an existing order product
        [HttpPut("{orderId}/{productId}")]
        public IActionResult UpdateOrderProduct(Guid orderId, Guid productId, [FromBody] OrderProduct updatedOrderProduct)
        {
            if (updatedOrderProduct == null || orderId == Guid.Empty || productId == Guid.Empty)
            {
                return BadRequest("Invalid order product data or IDs.");
            }

            var existingOrderProduct = _context.OrderProducts.SingleOrDefault(op => op.OrderId == orderId && op.ProductId == productId);
            if (existingOrderProduct == null)
            {
                return NotFound($"OrderProduct with OrderId {orderId} and ProductId {productId} not found.");
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

        // Delete an order product
        [HttpDelete("{orderId}/{productId}")]
        public IActionResult DeleteOrderProduct(Guid orderId, Guid productId)
        {
            var orderProduct = _context.OrderProducts.SingleOrDefault(op => op.OrderId == orderId && op.ProductId == productId);
            if (orderProduct == null)
            {
                return NotFound($"OrderProduct with OrderId {orderId} and ProductId {productId} not found.");
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
