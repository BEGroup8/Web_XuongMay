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
    public class ChuyenController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ChuyenController(MyDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả các Chuyen
        [HttpGet]
        public IActionResult GetAll()
        {
            var chuyens = _context.Chuyens.ToList();
            return Ok(chuyens);
        }

        // Lấy Chuyen theo ID
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var chuyen = _context.Chuyens.SingleOrDefault(c => c.ChuyenId == id);
            if (chuyen == null)
            {
                return NotFound($"Chuyen với ID {id} không tìm thấy.");
            }
            return Ok(chuyen);
        }

        // Tạo mới một Chuyen
        [HttpPost]
        public IActionResult Create([FromBody] Chuyen chuyenModel)
        {
            if (chuyenModel == null)
            {
                return BadRequest("Dữ liệu Chuyen bị null.");
            }

            var chuyen = new Chuyen
            {
                ChuyenId = Guid.NewGuid(),
                ChuyenName = chuyenModel.ChuyenName
            };

            _context.Chuyens.Add(chuyen);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = chuyen.ChuyenId }, chuyen);
        }

        // Cập nhật một Chuyen
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] Chuyen updatedChuyen)
        {
            if (updatedChuyen == null || id == Guid.Empty)
            {
                return BadRequest("Dữ liệu Chuyen hoặc ID không hợp lệ.");
            }

            var existingChuyen = _context.Chuyens.SingleOrDefault(c => c.ChuyenId == id);
            if (existingChuyen == null)
            {
                return NotFound($"Chuyen với ID {id} không tìm thấy.");
            }

            try
            {
                existingChuyen.ChuyenName = updatedChuyen.ChuyenName;

                _context.Chuyens.Update(existingChuyen);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Không thể cập nhật Chuyen. Lỗi: {ex.Message}");
            }
        }

        // Xóa một Chuyen
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var chuyen = _context.Chuyens.SingleOrDefault(c => c.ChuyenId == id);
            if (chuyen == null)
            {
                return NotFound($"Chuyen với ID {id} không tìm thấy.");
            }

            try
            {
                _context.Chuyens.Remove(chuyen);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Không thể xóa Chuyen. Lỗi: {ex.Message}");
            }
        }
    }
}
