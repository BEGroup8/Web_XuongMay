using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_XuongMay.Data;
using Web_XuongMay.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Web_XuongMay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ProductsController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var products = _context.Products.SingleOrDefault(p => p.MaHH == id);
            if (products == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            return Ok(products);
        }

        [HttpPost]
        public IActionResult Create(ProductsVM productVM)
        {
            var product = new Products
            {
                MaHH = Guid.NewGuid(),
                TenMH = productVM.TenHangHoa,
                MoTa = productVM.Mota,
                MaLoai = productVM.MaLoai // Ensure this is a valid ID for Loai
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = product.MaHH }, product);
        }
    }
}
