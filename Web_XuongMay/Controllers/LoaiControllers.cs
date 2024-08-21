using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Web_XuongMay.Data;
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

        // GET: api/Loai
        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Tổng số lượng Loai
                var totalRecords = _loaiRepository.GetAll().Count();

                // Lấy danh sách Loai với phân trang
                var dsLoai = _loaiRepository.GetAll()
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
                    Data = dsLoai
                };

                return Ok(paginationResult); // Trả về kết quả với HTTP 200 OK
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có vấn đề xảy ra
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // GET: api/Loai/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
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
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // POST: api/Loai
        [HttpPost]
        public IActionResult CreateNew([FromBody] LoaiModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.TenLoai))
            {
                return BadRequest("Invalid data. TenLoai is required.");
            }

            try
            {
                var loai = new Loai
                {
                    MaLoai = Guid.NewGuid(),
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

        // PUT: api/Loai/{id}
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] LoaiModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.TenLoai))
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

        // DELETE: api/Loai/{id}
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
