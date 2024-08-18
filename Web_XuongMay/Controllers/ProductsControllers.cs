using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_XuongMay.Data;
using Web_XuongMay.Models;

namespace Web_XuongMay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsControllers : ControllerBase
    {
        public static List<Products> hangHoas = new List<Products>();

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(hangHoas);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            try
            {
                // LINQ [Object] Query
                var hangHoa = hangHoas.SingleOrDefault(hh => hh.MaHH == Guid.Parse(id));
                if (hangHoa == null)
                {
                    return NotFound();
                }
                return Ok(hangHoa);
            }
            catch
            {
                return BadRequest();
            }
        }
        
        [HttpPost]
        public IActionResult Create(ProductsVM hangHoaVM)
        {
            var hanghoa = new Products
            {
                MaHH = Guid.NewGuid(),
                TenMH = hangHoaVM.TenHangHoa,
                MoTa = hangHoaVM.Mota
            };
            hangHoas.Add(hanghoa);
            return Ok(new
            {
                Success = true,
                Data = hanghoa
            });
        }
    }
}
