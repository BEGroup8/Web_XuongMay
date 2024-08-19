using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_XuongMay.Data;
using Web_XuongMay.Models;

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
        public IActionResult GetById(string id)
        {
            var productId = Guid.Parse(id);
            var product = _context.Products.SingleOrDefault(p => p.MaHH == productId);

            if (product == null)
            {
                return NotFound();
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
                MoTa = productVM.Mota
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(new
            {
                Success = true,
                Data = product
            });
        }
    }

}
