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

        public UserController(MyDbContext context, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpPost("Login")]
        public IActionResult Vadidate(LoginModel model)
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

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        // GET: api/User/{id}
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

        // POST: api/User
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

        // PUT: api/User/{id}
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

        // DELETE: api/User/{id}
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
