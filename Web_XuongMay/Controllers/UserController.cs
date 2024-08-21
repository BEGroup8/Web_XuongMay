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
        private readonly MyDbContext _context;
        private readonly AppSetting _appSettings;

        // Constructor with IOptionsMonitor for AppSettings
        public UserController(MyDbContext context, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
        }

        // Method to validate login
        [HttpPost("Login")]
        public IActionResult Validate(LoginModel model)
        {
            var user = _context.Users.SingleOrDefault(p => p.UserName == model.UserName && model.Password == p.Password);
            if (user == null)
            {
                return Ok(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Authenticate success",
                Data = GenerateToken(user)
            });
        }

        // Method to get all users with pagination
        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var totalRecords = _context.Users.Count();

                var users = _context.Users
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paginationResult = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                    Data = users
                };

                return Ok(paginationResult);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // Method to get user by ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(user);
        }

        // Method to create a new user
        [HttpPost]
        public IActionResult CreateNew([FromBody] UserModel userModel)
        {
            if (userModel == null)
            {
                return BadRequest("User data is null.");
            }

            try
            {
                var hashedPassword = HashPassword(userModel.Password);

                var user = new User
                {
                    UserName = userModel.UserName,
                    Password = hashedPassword,
                    FullName = userModel.FullName,
                    Email = userModel.Email
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok(new
                {
                    Success = true,
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create user. Error: {ex.Message}");
            }
        }

        // Method to update user by ID
        [HttpPut("{id}")]
        public IActionResult UpdateUserByID(int id, [FromBody] UserModel userModel)
        {
            if (userModel == null)
            {
                return BadRequest("Invalid user data or ID.");
            }

            var user = _context.Users.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            try
            {
                user.UserName = userModel.UserName;
                user.FullName = userModel.FullName;
                user.Email = userModel.Email;

                // Only update the password if it is provided
                if (!string.IsNullOrEmpty(userModel.Password))
                {
                    user.Password = HashPassword(userModel.Password);
                }

                _context.Users.Update(user);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update user. Error: {ex.Message}");
            }
        }

        // Method to delete user by ID
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            try
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to delete user. Error: {ex.Message}");
            }
        }

        // Helper method to hash passwords
        private string HashPassword(string password)
        {
            using (var hmac = new HMACSHA256())
            {
                var hashedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // Method to generate JWT token
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
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token);
        }
    }
}
