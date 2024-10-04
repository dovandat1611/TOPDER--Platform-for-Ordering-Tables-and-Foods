using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // API thêm mới thông báo
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationDto notificationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _notificationService.AddAsync(notificationDto);
            if (result)
            {
                return Ok("Thêm thông báo thành công.");
            }
            return BadRequest("Thêm thông báo thất bại.");
        }

        // API lấy thông tin chi tiết thông báo
        [HttpGet("{notificationId}")]
        public async Task<IActionResult> GetById(int notificationId, [FromQuery] int userId)
        {
            try
            {
                var notification = await _notificationService.GetItemAsync(notificationId, userId);
                return Ok(notification);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        // API lấy danh sách thông báo với phân trang
        [HttpGet("list")]
        public async Task<IActionResult> GetPaged(int pageNumber, int pageSize, [FromQuery] int userId)
        {
            var notifications = await _notificationService.GetPagingAsync(pageNumber, pageSize, userId);
            return Ok(notifications);
        }

        // API đánh dấu thông báo là đã đọc
        [HttpPut("IsRead/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _notificationService.IsReadAsync(id);
            if (result)
            {
                return Ok("Thông báo đã được đánh dấu là đã đọc.");
            }
            return NotFound("Không tìm thấy thông báo hoặc thông báo đã được đánh dấu là đã đọc.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] NotificationDto notificationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _notificationService.UpdateAsync(notificationDto);
            if (result)
            {
                return Ok("Cập nhật thông báo thành công.");
            }
            return NotFound("Không tìm thấy thông báo hoặc thông báo không thuộc về user.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int userId)
        {
            var result = await _notificationService.RemoveAsync(id, userId);
            if (result)
            {
                return Ok("Xóa thông báo thành công.");
            }
            return NotFound("Không tìm thấy thông báo hoặc thông báo không thuộc về user.");
        }
    }
}
