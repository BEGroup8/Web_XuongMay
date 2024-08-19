using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web_XuongMay.Models;
using Web_XuongMay.Services;

namespace Web_XuongMay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiControllers : ControllerBase
    {
        private readonly ILoaiRepository _loaiRepository;
        private readonly MyDbContext _context;

        public LoaiControllers(ILoaiRepository loaiRepository, MyDbContext context)
        {
            _loaiRepository = loaiRepository;
            _context = context;
        }

        // Get all Loai
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_loaiRepository.GetAll());
            }
            catch
            {
                return BadRequest();
            }
        }

        // Get Loai by Id
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)  // Sử dụng Guid cho id
        {
            try
            {
                var loai = _loaiRepository.GetById(id);
                if (loai != null)
                {
                    return Ok(loai);
                }
                else
                {
                    return NotFound();
                }
            }
            catch
            {
                return BadRequest();
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

                _loaiRepository.Add(loai);
                return CreatedAtAction(nameof(GetById), new { id = loai.MaLoai }, loai);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // Update Loai by Id
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] LoaiModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var loai = _loaiRepository.GetById(id);
                if (loai == null)
                {
                    return NotFound();
                }

                loai.TenLoai = model.TenLoai;
                _loaiRepository.Update(loai);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // Delete Loai by Id
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                var loai = _loaiRepository.GetById(id);
                if (loai == null)
                {
                    return NotFound();
                }

                _loaiRepository.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }
    }
}
