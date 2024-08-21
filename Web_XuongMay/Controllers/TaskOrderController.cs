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
                // Tổng số lượng Task trong cơ sở dữ liệu
                var totalRecords = _context.TaskOrders.Count();

                // Lấy danh sách Task với phân trang
                var tasks = _context.TaskOrders
                    .Include(t => t.Line) // Bao gồm cả thông tin của Line liên quan
                    .Include(t => t.OrderProduct) // Bao gồm cả thông tin của OrderProduct liên quan
                    .Skip((pageNumber - 1) * pageSize) // Bỏ qua các Task của trang trước
                    .Take(pageSize) // Lấy số lượng Task theo kích thước trang
                    .ToList();

                // Tạo object chứa thông tin phân trang
                var paginationResult = new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize), // Tính tổng số trang
                    Data = tasks // Dữ liệu Task cho trang hiện tại
                };

                // Trả về dữ liệu với HTTP 200 OK
                return Ok(paginationResult);
            }
            catch (Exception ex)
            {
                // Nếu xảy ra lỗi, trả về HTTP 400 Bad Request với thông báo lỗi
                return BadRequest($"Error occurred: {ex.Message}");
            }
        }

        // Lấy Task theo ID
        [HttpGet("{taskId}")]
        public IActionResult GetById(Guid taskId)
        {
            // Tìm Task theo TaskId
            var task = _context.TaskOrders
                .Include(t => t.Line) // Bao gồm cả thông tin của Line liên quan
                .Include(t => t.OrderProduct) // Bao gồm cả thông tin của OrderProduct liên quan
                .SingleOrDefault(t => t.TaskId == taskId);

            if (task == null)
            {
                // Nếu không tìm thấy Task, trả về HTTP 404 Not Found
                return NotFound($"Task với ID {taskId} không tìm thấy.");
            }

            // Nếu tìm thấy, trả về đối tượng Task với HTTP 200 OK
            return Ok(task);
        }

        // Tạo mới một Task
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskModel taskModel)
        {
            if (taskModel == null)
            {
                // Nếu dữ liệu đầu vào bị null, trả về HTTP 400 Bad Request
                return BadRequest("Dữ liệu Task bị null.");
            }

            // Chuyển đổi TaskModel sang TaskOrder
            var task = new TaskOrder
            {
                TaskId = Guid.NewGuid(), // Tạo ID mới cho Task
                TaskName = taskModel.TaskName,
                LineId = taskModel.LineId,
                OrderProductId = taskModel.OrderProductId
            };

            // Thêm Task mới vào DbContext
            _context.TaskOrders.Add(task);

            try
            {
                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Trả về HTTP 201 Created với thông tin của Task mới được tạo
                return CreatedAtAction(nameof(GetById), new { taskId = task.TaskId }, task);
            }
            catch (DbUpdateException dbEx)
            {
                // Nếu xảy ra lỗi cơ sở dữ liệu, trả về HTTP 400 Bad Request với thông báo lỗi
                var sqlError = dbEx.InnerException?.Message ?? dbEx.Message;
                return BadRequest($"Không thể tạo Task. Lỗi cơ sở dữ liệu: {sqlError}");
            }
            catch (Exception ex)
            {
                // Nếu xảy ra lỗi khác, trả về HTTP 400 Bad Request với thông báo lỗi
                return BadRequest($"Không thể tạo Task. Lỗi: {ex.Message}");
            }
        }

        // Cập nhật một Task
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] TaskModel updatedTaskModel)
        {
            if (updatedTaskModel == null || taskId == Guid.Empty)
            {
                // Nếu dữ liệu đầu vào bị null hoặc ID không hợp lệ, trả về HTTP 400 Bad Request
                return BadRequest("Dữ liệu Task hoặc ID không hợp lệ.");
            }

            // Tìm đối tượng Task theo ID
            var existingTask = _context.TaskOrders.SingleOrDefault(t => t.TaskId == taskId);
            if (existingTask == null)
            {
                // Nếu không tìm thấy Task, trả về HTTP 404 Not Found
                return NotFound($"Task với ID {taskId} không tìm thấy.");
            }

            try
            {
                // Cập nhật thông tin Task
                existingTask.TaskName = updatedTaskModel.TaskName;
                existingTask.LineId = updatedTaskModel.LineId;
                existingTask.OrderProductId = updatedTaskModel.OrderProductId;

                // Cập nhật đối tượng trong DbContext
                _context.TaskOrders.Update(existingTask);
                await _context.SaveChangesAsync();

                // Trả về HTTP 204 No Content nếu cập nhật thành công
                return NoContent();
            }
            catch (Exception ex)
            {
                // Nếu xảy ra lỗi khi cập nhật Task, trả về HTTP 400 Bad Request với thông báo lỗi
                return BadRequest($"Không thể cập nhật Task. Lỗi: {ex.Message}");
            }
        }

        // Xóa một Task
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            // Tìm đối tượng Task theo ID
            var task = _context.TaskOrders.SingleOrDefault(t => t.TaskId == taskId);
            if (task == null)
            {
                // Nếu không tìm thấy Task, trả về HTTP 404 Not Found
                return NotFound($"Task với ID {taskId} không tìm thấy.");
            }

            try
            {
                // Xóa Task khỏi DbContext
                _context.TaskOrders.Remove(task);
                await _context.SaveChangesAsync();

                // Trả về HTTP 204 No Content nếu xóa thành công
                return NoContent();
            }
            catch (Exception ex)
            {
                // Nếu xảy ra lỗi khi xóa Task, trả về HTTP 400 Bad Request với thông báo lỗi
                return BadRequest($"Không thể xóa Task. Lỗi: {ex.Message}");
            }
        }
    }
}
