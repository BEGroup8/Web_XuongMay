using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using Web_XuongMay.Data;
using Web_XuongMay.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Web_XuongMay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context; // Đối tượng DbContext để làm việc với cơ sở dữ liệu
        private readonly AppSetting _appSettings; // Đối tượng chứa các cấu hình từ AppSettings

        // Constructor với IOptionsMonitor để lấy cấu hình từ AppSettings
        public UserController(MyDbContext context, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _context = context; // Khởi tạo DbContext
            _appSettings = optionsMonitor.CurrentValue; // Khởi tạo cấu hình ứng dụng
        }

        // Phương thức để xác thực đăng nhập
        [HttpPost("Login")]
        public IActionResult Validate(LoginModel model)
        {
            // Kiểm tra xem người dùng có tồn tại và mật khẩu có khớp không
            var user = _context.Users.SingleOrDefault(p => p.UserName == model.UserName && model.Password == p.Password);
            if (user == null)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid username or password" // Thông báo lỗi nếu thông tin đăng nhập không hợp lệ
                });
            }

            // Trả về kết quả thành công và JWT token nếu đăng nhập thành công
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Authenticate success",
                Data = GenerateToken(user) // Tạo và trả về JWT token
            });
        }

        // Phương thức để lấy tất cả người dùng với phân trang
        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Tính tổng số bản ghi người dùng
                var totalRecords = _context.Users.Count();

                // Lấy danh sách người dùng theo phân trang
                var users = _context.Users
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Tạo đối tượng chứa dữ liệu phân trang
                var paginationResult = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                    Data = users
                };

                return Ok(paginationResult); // Trả về kết quả với HTTP 200 OK
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}"); // Trả về lỗi nếu có xảy ra ngoại lệ
            }
        }

        // Phương thức để lấy thông tin người dùng theo ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            // Tìm người dùng theo ID
            var user = _context.Users.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found."); // Trả về lỗi 404 nếu không tìm thấy người dùng
            }
            return Ok(user); // Trả về thông tin người dùng nếu tìm thấy
        }

        // Phương thức để tạo một người dùng mới
        [HttpPost]
        public IActionResult CreateNew([FromBody] UserModel userModel)
        {
            if (userModel == null)
            {
                return BadRequest("User data is null."); // Trả về lỗi nếu dữ liệu người dùng bị null
            }

            try
            {
                // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
                var hashedPassword = HashPassword(userModel.Password);

                var user = new User
                {
                    UserName = userModel.UserName,
                    Password = hashedPassword,
                    FullName = userModel.FullName,
                    Email = userModel.Email
                };

                // Thêm người dùng vào cơ sở dữ liệu và lưu thay đổi
                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok(new
                {
                    Success = true,
                    Data = user // Trả về thông tin người dùng mới tạo
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create user. Error: {ex.Message}"); // Trả về lỗi nếu không thể tạo người dùng
            }
        }

        // Phương thức để cập nhật thông tin người dùng theo ID
        [HttpPut("{id}")]
        public IActionResult UpdateUserByID(int id, [FromBody] UserModel userModel)
        {
            if (userModel == null)
            {
                return BadRequest("Invalid user data or ID."); // Trả về lỗi nếu dữ liệu người dùng hoặc ID không hợp lệ
            }

            // Tìm người dùng theo ID
            var user = _context.Users.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found."); // Trả về lỗi 404 nếu không tìm thấy người dùng
            }

            try
            {
                // Cập nhật thông tin người dùng
                user.UserName = userModel.UserName;
                user.FullName = userModel.FullName;
                user.Email = userModel.Email;

                // Chỉ cập nhật mật khẩu nếu nó được cung cấp
                if (!string.IsNullOrEmpty(userModel.Password))
                {
                    user.Password = HashPassword(userModel.Password);
                }

                _context.Users.Update(user);
                _context.SaveChanges();
                return NoContent(); // Trả về HTTP 204 No Content nếu cập nhật thành công
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update user. Error: {ex.Message}"); // Trả về lỗi nếu không thể cập nhật người dùng
            }
        }

        // Phương thức để xóa người dùng theo ID
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            // Tìm người dùng theo ID
            var user = _context.Users.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found."); // Trả về lỗi 404 nếu không tìm thấy người dùng
            }

            try
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return NoContent(); // Trả về HTTP 204 No Content nếu xóa thành công
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete user. Error: {ex.Message}"); // Trả về lỗi nếu không thể xóa người dùng
            }
        }

        // Phương thức trợ giúp để mã hóa mật khẩu
        private string HashPassword(string password)
        {
            using (var hmac = new HMACSHA256())
            {
                var hashedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes); // Trả về mật khẩu đã mã hóa dưới dạng chuỗi base64
            }
        }

        // Phương thức để tạo JWT token
        private string GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
            var tokenDescription = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserName", user.UserName),
                    new Claim("Id", user.Id.ToString()),
                    new Claim("TokenId", Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(1), // Thời gian hết hạn của token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token); // Trả về token dưới dạng chuỗi
        }
    }
}
