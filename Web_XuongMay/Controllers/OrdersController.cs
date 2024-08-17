using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_XuongMay.Data;
using Web_XuongMay.Models;

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
            var listOrder = _context.Orders.ToList();
            return Ok(listOrder);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateOrderByID(Guid id,OrderModel ordermodel)
        {
            var order = _context.Orders.SingleOrDefault(o => o.Id == id);
            if (order != null)
            {
                order.OrderNumber = ordermodel.OrderNumber;
                order.OrderDate = ordermodel.OrderDate;
                order.TotalAmount = ordermodel.TotalAmount;
                order.OrderProducts = ordermodel.OrderProducts;
                _context.SaveChanges();
                return NoContent();
            }    
            else
            {
                return NotFound();
            }    
        }
        [HttpPost]
        public IActionResult CreateNew(OrderModel orderModel)
        {
            try
            {
                var order = new Order
                {
                    OrderNumber = orderModel.OrderNumber,
                    OrderDate = orderModel.OrderDate,
                    TotalAmount = orderModel.TotalAmount,
                    OrderProducts = orderModel.OrderProducts
                };
                _context.Add(order);
                _context.SaveChanges();
                return Ok(order);
            }
            catch 
            {
                return BadRequest();
            }
            

        }
    }
}
