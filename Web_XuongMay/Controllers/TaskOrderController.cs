using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web_XuongMay.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web_XuongMay.Models;

namespace Web_XuongMay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskOrderController : ControllerBase
    {
        private readonly MyDbContext _context;

        public TaskOrderController(MyDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả các Task với phân trang
        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Tổng số lượng Task
                var totalRecords = _context.TaskOrders.Count();

                // Lấy danh sách Task với phân trang
                var tasks = _context.TaskOrders
                    .Include(t => t.Line)
                    .Include(t => t.OrderProduct)
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
                    Data = tasks
                };

                return Ok(paginationResult); // Trả về kết quả với HTTP 200 OK
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // Lấy Task theo ID
        [HttpGet("{taskId}")]
        public IActionResult GetById(Guid taskId)
        {
            var task = _context.TaskOrders
                .Include(t => t.Line)
                .Include(t => t.OrderProduct)
                .SingleOrDefault(t => t.TaskId == taskId);

            if (task == null)
            {
                return NotFound($"Task với ID {taskId} không tìm thấy.");
            }

            return Ok(task);
        }

        // Tạo mới một Task
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskModel taskModel)
        {
            if (taskModel == null)
            {
                return BadRequest("Dữ liệu Task bị null.");
            }

            // Chuyển đổi TaskModel sang Task
            var task = new TaskOrder
            {
                TaskId = Guid.NewGuid(), // Tạo ID mới cho Task
                TaskName = taskModel.TaskName,
                LineId = taskModel.LineId,
                OrderProductId = taskModel.OrderProductId
            };

            _context.TaskOrders.Add(task);

            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { taskId = task.TaskId }, task);
            }
            catch (DbUpdateException dbEx)
            {
                var sqlError = dbEx.InnerException?.Message ?? dbEx.Message;
                return BadRequest($"Không thể tạo Task. Lỗi cơ sở dữ liệu: {sqlError}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Không thể tạo Task. Lỗi: {ex.Message}");
            }
        }

        // Cập nhật một Task
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] TaskModel updatedTaskModel)
        {
            if (updatedTaskModel == null || taskId == Guid.Empty)
            {
                return BadRequest("Dữ liệu Task hoặc ID không hợp lệ.");
            }

            var existingTask = _context.TaskOrders.SingleOrDefault(t => t.TaskId == taskId);
            if (existingTask == null)
            {
                return NotFound($"Task với ID {taskId} không tìm thấy.");
            }

            try
            {
                existingTask.TaskName = updatedTaskModel.TaskName;
                existingTask.LineId = updatedTaskModel.LineId;
                existingTask.OrderProductId = updatedTaskModel.OrderProductId;

                _context.TaskOrders.Update(existingTask);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Không thể cập nhật Task. Lỗi: {ex.Message}");
            }
        }

        // Xóa một Task
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            var task = _context.TaskOrders.SingleOrDefault(t => t.TaskId == taskId);
            if (task == null)
            {
                return NotFound($"Task với ID {taskId} không tìm thấy.");
            }

            try
            {
                _context.TaskOrders.Remove(task);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Không thể xóa Task. Lỗi: {ex.Message}");
            }
        }
    }
}
