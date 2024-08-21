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
    // Đánh dấu class này là một API controller và thiết lập route mặc định cho nó là "api/Products".
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly MyDbContext _context;

        // Constructor để inject MyDbContext vào trong controller.
        public ProductsController(MyDbContext context)
        {
            _context = context;
        }

        // Action để lấy tất cả sản phẩm trong cơ sở dữ liệu.
        // GET: api/Products
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _context.Products.ToList(); // Lấy tất cả sản phẩm từ database.
            return Ok(products); // Trả về danh sách sản phẩm với mã trạng thái 200 (OK).
            IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
            {
                try
                {
                    // Tổng số lượng Products
                    var totalRecords = _context.Products.Count();

                    // Lấy danh sách Products với phân trang
                    var products = _context.Products
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
                        Data = products
                    };

                    return Ok(paginationResult); // Trả về kết quả với HTTP 200 OK
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error occurred: {ex.Message}");
                }
            }
        }

            // Action để lấy sản phẩm theo ID.
            // GET: api/Products/{id}
            [HttpGet("{id}")]
            public IActionResult GetById(Guid id)
            {
                var product = _context.Products.SingleOrDefault(p => p.MaHH == id); // Tìm sản phẩm theo mã sản phẩm (MaHH).
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found."); // Nếu không tìm thấy, trả về mã trạng thái 404 (Not Found).
                }
                return Ok(product); // Nếu tìm thấy, trả về sản phẩm với mã trạng thái 200 (OK).
            }

            // Action để tạo mới một sản phẩm.
            // POST: api/Products
            [HttpPost]
          public IActionResult Create(ProductsVM productVM)
            {
                var product = new Products
                {
                    MaHH = Guid.NewGuid(), // Tạo mới một GUID cho sản phẩm.
                    TenMH = productVM.TenHangHoa, // Lấy tên hàng hóa từ ProductsVM.
                    MoTa = productVM.Mota, // Lấy mô tả từ ProductsVM.
                    MaLoai = productVM.MaLoai // Gán mã loại.
                };

                _context.Products.Add(product); // Thêm sản phẩm mới vào context.
                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu.

                // Trả về mã trạng thái 201 (Created) và URL để truy cập sản phẩm vừa tạo.
                return CreatedAtAction(nameof(GetById), new { id = product.MaHH }, product);
            }

            // Action để chỉnh sửa một sản phẩm.
            // PUT: api/Products/{id}
            [HttpPut("{id}")]
            public IActionResult Edit(Guid id, Products hangHoaEdit)
            {
                try
                {
                    var hangHoa = _context.Products.SingleOrDefault(hh => hh.MaHH == id); // Tìm sản phẩm theo mã sản phẩm.
                    if (hangHoa == null)
                    {
                        return NotFound(); // Nếu không tìm thấy, trả về mã trạng thái 404 (Not Found).
                    }

                    // Cập nhật thông tin sản phẩm.
                    hangHoa.TenMH = hangHoaEdit.TenMH;
                    hangHoa.MoTa = hangHoaEdit.MoTa;

                    _context.Products.Update(hangHoa); // Cập nhật sản phẩm trong context.
                    _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu.

                    return Ok(); // Trả về mã trạng thái 200 (OK) nếu thành công.
                }
                catch
                {
                    return BadRequest(); // Trả về mã trạng thái 400 (Bad Request) nếu có lỗi xảy ra.
                }
            }

            // Action để xóa một sản phẩm.
            // DELETE: api/Products/{id}
            [HttpDelete("{id}")]
            public IActionResult Remove(Guid id)
            {
                try
                {
                    var hangHoa = _context.Products.SingleOrDefault(hh => hh.MaHH == id); // Tìm sản phẩm theo mã sản phẩm.
                    if (hangHoa == null)
                    {
                        return NotFound(); // Nếu không tìm thấy, trả về mã trạng thái 404 (Not Found).
                    }

                    _context.Products.Remove(hangHoa); // Xóa sản phẩm khỏi context.
                    _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu.

                    return Ok(); // Trả về mã trạng thái 200 (OK) nếu xóa thành công.
                }
                catch
                {
                    return BadRequest(); // Trả về mã trạng thái 400 (Bad Request) nếu có lỗi xảy ra.
                }
            }
        
    }
}

