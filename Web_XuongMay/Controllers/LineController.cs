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

        // Get all Lines
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var dsLines = _context.Lines.ToList();
                return Ok(dsLines);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // Get Line by Id
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var line = _context.Lines.SingleOrDefault(lo => lo.LineId == id);
                if (line != null)
                {
                    return Ok(line);
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

        // Create a new Line
        [HttpPost]
        public IActionResult CreateNew([FromBody] LineModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.NameLine))
            {
                return BadRequest("Invalid data. NameLine is required.");
            }

            // Kiểm tra UserId có hợp lệ không
            var user = _context.Users.SingleOrDefault(u => u.Id == model.UserId);
            if (user == null)
            {
                return BadRequest("Invalid UserId. The user does not exist.");
            }

            try
            {
                var line = new Line
                {
                    LineId = Guid.NewGuid(),
                    NameLine = model.NameLine,
                    Id = model.UserId // Gán UserId cho Line
                };

                _context.Lines.Add(line);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetById), new { id = line.LineId }, line);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred while saving the entity changes: {ex.Message}");
            }
        }


        // Update Line by Id
        [HttpPut("{id}")]
        public IActionResult UpdateLineById(Guid id, [FromBody] LineModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var line = _context.Lines.SingleOrDefault(lo => lo.LineId == id);
                if (line != null)
                {
                    line.NameLine = model.NameLine;
                    _context.SaveChanges();
                    return NoContent();
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

        // Delete Line by Id
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                var line = _context.Lines.SingleOrDefault(lo => lo.LineId == id);
                if (line != null)
                {
                    _context.Lines.Remove(line);
                    _context.SaveChanges();
                    return NoContent();
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
    }
}