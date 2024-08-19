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
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiControllers : ControllerBase
    {
        private readonly MyDbContext _context;

        public LoaiControllers(MyDbContext context)
        {
            _context = context;
        }

        // Get all Loai
        [HttpGet]
        public IActionResult GetAll()
        {
            var dsLoai = _context.Loais.ToList();
            return Ok(dsLoai);
        }

        // Get Loai by Id
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)  // Sử dụng Guid cho id
        {
            var loai = _context.Loais.SingleOrDefault(lo => lo.MaLoai == id);
            if (loai != null)
            {
                return Ok(loai);
            }
            else
            {
                return NotFound();
            }
        }

        // Create a new Loai
        [HttpPost]
        public IActionResult CreateNew([FromBody] LoaiModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var loai = new Loai
                {
                    MaLoai = Guid.NewGuid(),  // Tạo Guid mới cho khóa chính
                    TenLoai = model.TenLoai
                };

                _context.Loais.Add(loai);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetById), new { id = loai.MaLoai }, loai);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // Update Loai by Id
        [HttpPut("{id}")]
        public IActionResult UpdateLoaiById(Guid id, [FromBody] LoaiModel model)  // Sử dụng Guid cho id
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var loai = _context.Loais.SingleOrDefault(lo => lo.MaLoai == id);
            if (loai != null)
            {
                loai.TenLoai = model.TenLoai;

                try
                {
                    _context.SaveChanges();
                    return NoContent();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error occurred: {ex.Message}");
                }
            }
            else
            {
                return NotFound();
            }
        }

        // Delete Loai by Id
        [HttpDelete("{id}")]
        public IActionResult DeleteLoaiById(Guid id)  // Sử dụng Guid cho id
        {
            var loai = _context.Loais.SingleOrDefault(lo => lo.MaLoai == id);
            if (loai != null)
            {
                try
                {
                    _context.Loais.Remove(loai);
                    _context.SaveChanges();
                    return NoContent();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error occurred: {ex.Message}");
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}
