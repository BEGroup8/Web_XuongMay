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

        [HttpGet]
        public IActionResult GetAll()
        {
            var orders = _context.Orders.ToList();
            return Ok(orders);
        }

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
