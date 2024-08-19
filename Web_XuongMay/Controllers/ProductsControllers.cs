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
            var product = _context.Products.SingleOrDefault(p => p.MaHH == id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(product);
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

        [HttpPut("{id}")]
        public IActionResult Edit(Guid id, Products hangHoaEdit)
        {
            try
            {
                var hangHoa = _context.Products.SingleOrDefault(hh => hh.MaHH == id);
                if (hangHoa == null)
                {
                    return NotFound();
                }

                // Update
                hangHoa.TenMH = hangHoaEdit.TenMH;
                hangHoa.MoTa = hangHoaEdit.MoTa;

                _context.Products.Update(hangHoa);
                _context.SaveChanges();

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(Guid id)
        {
            try
            {
                var hangHoa = _context.Products.SingleOrDefault(hh => hh.MaHH == id);
                if (hangHoa == null)
                {
                    return NotFound();
                }

                _context.Products.Remove(hangHoa);
                _context.SaveChanges();

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
