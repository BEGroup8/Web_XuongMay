using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Web_XuongMay.Data;
using Web_XuongMay.Models;

namespace Web_XuongMay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineController : Controller
    {
        private readonly MyDbContext _context;

        public LineController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Line
        // Phương thức này trả về danh sách tất cả các Line từ cơ sở dữ liệu với phân trang
        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Tổng số lượng Line
                var totalRecords = _context.Lines.Count();

                // Lấy danh sách Line với phân trang
                var dsLines = _context.Lines
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
                    Data = dsLines
                };

                return Ok(paginationResult); // Trả về kết quả với HTTP 200 OK
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có vấn đề xảy ra
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // GET: api/Line/{id}
        // Phương thức này trả về một Line theo ID
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                // Tìm Line theo ID
                var line = _context.Lines.SingleOrDefault(lo => lo.LineId == id);
                if (line != null)
                {
                    return Ok(line); // Nếu tìm thấy, trả về Line dưới dạng HTTP 200 OK
                }
                else
                {
                    return NotFound(); // Nếu không tìm thấy, trả về HTTP 404 Not Found
                }
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có vấn đề xảy ra
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // POST: api/Line
        // Phương thức này tạo một Line mới
        [HttpPost]
        public IActionResult CreateNew([FromBody] LineModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.NameLine))
            {
                return BadRequest("Invalid data. NameLine is required.");
            }

            // Kiểm tra nếu UserId hợp lệ
            var user = _context.Users.SingleOrDefault(u => u.Id == model.UserId);
            if (user == null)
            {
                return BadRequest("Invalid UserId. The user does not exist."); // Trả về lỗi nếu UserId không hợp lệ
            }

            try
            {
                // Tạo một Line mới với dữ liệu từ model
                var line = new Line
                {
                    LineId = Guid.NewGuid(), // Tạo mới một GUID cho LineId
                    NameLine = model.NameLine,
                    Id = model.UserId // Gán UserId cho Line
                };

                // Thêm Line vào cơ sở dữ liệu
                _context.Lines.Add(line);
                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                // Trả về HTTP 201 Created với thông tin của Line vừa tạo
                return CreatedAtAction(nameof(GetById), new { id = line.LineId }, line);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có vấn đề xảy ra khi lưu dữ liệu
                return BadRequest($"Error occurred while saving the entity changes: {ex.Message}");
            }
        }

        // PUT: api/Line/{id}
        // Phương thức này cập nhật thông tin của một Line theo ID
        [HttpPut("{id}")]
        public IActionResult UpdateLineById(Guid id, [FromBody] LineModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.NameLine))
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                // Tìm Line theo ID
                var line = _context.Lines.SingleOrDefault(lo => lo.LineId == id);
                if (line != null)
                {
                    // Cập nhật NameLine của Line
                    line.NameLine = model.NameLine;
                    _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    return NoContent(); // Trả về HTTP 204 No Content để báo cáo thành công
                }
                else
                {
                    return NotFound(); // Nếu không tìm thấy Line, trả về HTTP 404 Not Found
                }
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có vấn đề xảy ra
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // DELETE: api/Line/{id}
        // Phương thức này xóa một Line theo ID
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                // Tìm Line theo ID
                var line = _context.Lines.SingleOrDefault(lo => lo.LineId == id);
                if (line != null)
                {
                    // Xóa Line khỏi cơ sở dữ liệu
                    _context.Lines.Remove(line);
                    _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    return NoContent(); // Trả về HTTP 204 No Content để báo cáo thành công
                }
                else
                {
                    return NotFound(); // Nếu không tìm thấy Line, trả về HTTP 404 Not Found
                }
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có vấn đề xảy ra
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }
    }
}
