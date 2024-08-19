using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_XuongMay.Data;
using Web_XuongMay.Models;

namespace Web_XuongMay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaisControllers : ControllerBase
    {
        private readonly MyDbContext _context;

        public LoaisControllers(MyDbContext context)
        {
            _context = context;

        }
        [HttpGet]
        public IActionResult GetAll()

        {
            var dsLoai = _context.Loais.ToList();
            return Ok(dsLoai);

        }
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)

        {
            var loai = _context.Loais.SingleOrDefault(lo =>
            lo.MaLoai == id);
            if (loai != null)
            {
                return Ok(loai);
            }
            else
            {
                return NotFound();
            }

        }
        [HttpPost]
        [Authorize]
        public IActionResult CreateNew(LoaiModel model)
        {
            try
            {
                var loai = new Loai
                {
                    TenLoai = model.TenLoai,
                };

                _context.Add(model);
                _context.SaveChanges();
                return Ok(loai);
            }
            catch
            { return BadRequest(0); 
            }
        }



        [HttpPut("{id}")]
        public IActionResult UpdateLoaiById(Guid id, LoaiModel model)

        {
            var loai = _context.Loais.SingleOrDefault(lo =>
            lo.MaLoai == id);
            if (loai != null)
            {
                loai.TenLoai = model.TenLoai;
                _context.SaveChanges();
                return NoContent();
            }
            else
            {
                return NotFound();

            }
        }
       

    }
}
