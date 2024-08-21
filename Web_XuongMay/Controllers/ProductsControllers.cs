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

        // GET: api/Products
        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Tổng số lượng Products
                var totalRecords = _context.Products.Count();

                // Lấy danh sách Products với phân trang
                var products = _context.Products
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
                    Data = products
                };

                return Ok(paginationResult); // Trả về kết quả với HTTP 200 OK
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // GET: api/Products/{id}
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

        // POST: api/Products
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

        // PUT: api/Products/{id}
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

        // DELETE: api/Products/{id}
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
