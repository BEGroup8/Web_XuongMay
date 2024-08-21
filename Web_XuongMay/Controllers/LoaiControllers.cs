using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Web_XuongMay.Data;
using Web_XuongMay.Models;
using Web_XuongMay.Services;

namespace Web_XuongMay.Controllers
{
    // Định nghĩa route API cho controller này là "api/Loai"
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiControllers : ControllerBase
    {
        private readonly ILoaiRepository _loaiRepository;
        private readonly MyDbContext _context;

        // Constructor để inject repository và context vào controller
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
                // Tổng số lượng Loai trong cơ sở dữ liệu
                var totalRecords = _loaiRepository.GetAll().Count();

                // Lấy danh sách Loai với phân trang
                var dsLoai = _loaiRepository.GetAll()
                    .Skip((pageNumber - 1) * pageSize) // Bỏ qua các bản ghi đã được phân trang
                    .Take(pageSize) // Lấy một số lượng bản ghi giới hạn bởi pageSize
                    .ToList();

                // Tạo object chứa dữ liệu phân trang
                var paginationResult = new
                {
                    PageNumber = pageNumber, // Số trang hiện tại
                    PageSize = pageSize, // Số lượng bản ghi trên mỗi trang
                    TotalRecords = totalRecords, // Tổng số bản ghi
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize), // Tổng số trang
                    Data = dsLoai // Dữ liệu của các bản ghi trong trang hiện tại
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
                // Tìm đối tượng Loai theo ID
                var loai = _loaiRepository.GetById(id);
                if (loai != null)
                {
                    return Ok(loai); // Trả về đối tượng nếu tìm thấy
                }
                else
                {
                    return NotFound(); // Trả về HTTP 404 nếu không tìm thấy
                }
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có vấn đề xảy ra
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // POST: api/Loai
        [HttpPost]
        public IActionResult CreateNew([FromBody] LoaiModel model)
        {
            // Kiểm tra tính hợp lệ của dữ liệu
            if (model == null || string.IsNullOrWhiteSpace(model.TenLoai))
            {
                return BadRequest("Invalid data. TenLoai is required."); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            try
            {
                // Tạo đối tượng Loai mới
                var loai = new Loai
                {
                    MaLoai = Guid.NewGuid(), // Tạo GUID mới cho Loai
                    TenLoai = model.TenLoai // Gán giá trị TenLoai từ model
                };

                _loaiRepository.Add(loai); // Thêm đối tượng mới vào repository

                // Trả về HTTP 201 Created với URL truy cập đối tượng vừa tạo
                return CreatedAtAction(nameof(GetById), new { id = loai.MaLoai }, loai);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có vấn đề xảy ra
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // PUT: api/Loai/{id}
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] LoaiModel model)
        {
            // Kiểm tra tính hợp lệ của dữ liệu
            if (model == null || string.IsNullOrWhiteSpace(model.TenLoai))
            {
                return BadRequest("Invalid data."); // Trả về lỗi nếu dữ liệu không hợp lệ
            }

            try
            {
                // Tìm đối tượng Loai theo ID
                var loai = _loaiRepository.GetById(id);
                if (loai == null)
                {
                    return NotFound(); // Trả về HTTP 404 nếu không tìm thấy
                }

                // Cập nhật thông tin Loai
                loai.TenLoai = model.TenLoai;
                _loaiRepository.Update(loai); // Cập nhật đối tượng trong repository

                return NoContent(); // Trả về HTTP 204 No Content nếu cập nhật thành công
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có vấn đề xảy ra
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // DELETE: api/Loai/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                // Tìm đối tượng Loai theo ID
                var loai = _loaiRepository.GetById(id);
                if (loai == null)
                {
                    return NotFound(); // Trả về HTTP 404 nếu không tìm thấy
                }

                _loaiRepository.Delete(id); // Xóa đối tượng trong repository
                return NoContent(); // Trả về HTTP 204 No Content nếu xóa thành công
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có vấn đề xảy ra
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }
    }
}
