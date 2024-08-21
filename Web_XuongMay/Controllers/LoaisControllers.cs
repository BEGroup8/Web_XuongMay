using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_XuongMay.Data;
using Web_XuongMay.Models;

namespace Web_XuongMay.Controllers
{
    // Định nghĩa route API cho controller này là "api/Loais"
    [Route("api/[controller]")]
    [ApiController]
    public class LoaisControllers : ControllerBase
    {
        private readonly MyDbContext _context;

        // Constructor để inject DbContext vào controller
        public LoaisControllers(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Loais
        [HttpGet]
        public IActionResult GetAll()
        {
            // Lấy danh sách tất cả các đối tượng Loai từ cơ sở dữ liệu
            var dsLoai = _context.Loais.ToList();
            return Ok(dsLoai); // Trả về danh sách các Loai với HTTP 200 OK
        }

        // GET: api/Loais/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // Tìm đối tượng Loai theo ID
            var loai = _context.Loais.SingleOrDefault(lo => lo.MaLoai == id);
            if (loai != null)
            {
                return Ok(loai); // Trả về đối tượng nếu tìm thấy
            }
            else
            {
                return NotFound(); // Trả về HTTP 404 nếu không tìm thấy
            }
        }

        // POST: api/Loais
        [HttpPost]
        [Authorize] // Yêu cầu xác thực để truy cập endpoint này
        public IActionResult CreateNew(LoaiModel model)
        {
            try
            {
                // Tạo đối tượng Loai mới từ dữ liệu của model
                var loai = new Loai
                {
                    TenLoai = model.TenLoai,
                };

                // Thêm đối tượng mới vào DbContext
                _context.Add(loai);
                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                return Ok(loai); // Trả về đối tượng vừa được tạo với HTTP 200 OK
            }
            catch
            {
                return BadRequest("Error occurred while creating Loai."); // Trả về HTTP 400 nếu có lỗi
            }
        }

        // PUT: api/Loais/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateLoaiById(Guid id, LoaiModel model)
        {
            // Tìm đối tượng Loai theo ID
            var loai = _context.Loais.SingleOrDefault(lo => lo.MaLoai == id);
            if (loai != null)
            {
                // Cập nhật thông tin của đối tượng Loai
                loai.TenLoai = model.TenLoai;
                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                return NoContent(); // Trả về HTTP 204 No Content nếu cập nhật thành công
            }
            else
            {
                return NotFound(); // Trả về HTTP 404 nếu không tìm thấy đối tượng
            }
        }
    }
}
