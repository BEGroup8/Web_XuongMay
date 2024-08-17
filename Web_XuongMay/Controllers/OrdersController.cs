using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_XuongMay.Data;
using Web_XuongMay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // Read all orders
        [HttpGet]
        public IActionResult GetAll()
        {
            var orders = _context.Orders.Include(o => o.OrderProducts).ToList();
            return Ok(orders);
        }

        // Read a specific order by ID
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var order = _context.Orders.Include(o => o.OrderProducts)
                                       .SingleOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok(order);
        }

        // Create a new order
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
                    Id = Guid.NewGuid(),
                    OrderNumber = orderModel.OrderNumber,
                    OrderDate = orderModel.OrderDate,
                    TotalAmount = orderModel.TotalAmount,
                    OrderProducts = orderModel.OrderProducts.Select(op => new OrderProduct
                    {
                        ProductId = op.ProductId,
                        Quantity = op.Quantity
                    }).ToList()
                };

                _context.Orders.Add(order);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create order. Error: {ex.Message}");
            }
        }

        // Update an existing order by ID
        [HttpPut("{id}")]
        public IActionResult UpdateOrderByID(Guid id, [FromBody] OrderModel orderModel)
        {
            if (orderModel == null || id == Guid.Empty)
            {
                return BadRequest("Invalid order data or ID.");
            }

            var order = _context.Orders.Include(o => o.OrderProducts)
                                       .SingleOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            try
            {
                order.OrderNumber = orderModel.OrderNumber;
                order.OrderDate = orderModel.OrderDate;
                order.TotalAmount = orderModel.TotalAmount;

                // Update OrderProducts
                _context.OrderProducts.RemoveRange(order.OrderProducts);
                order.OrderProducts = orderModel.OrderProducts.Select(op => new OrderProduct
                {
                    OrderId = id,
                    ProductId = op.ProductId,
                    Quantity = op.Quantity
                }).ToList();

                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update order. Error: {ex.Message}");
            }
        }

        // Delete an order by ID
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(Guid id)
        {
            var order = _context.Orders.Include(o => o.OrderProducts)
                                       .SingleOrDefault(o => o.Id == id);
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
