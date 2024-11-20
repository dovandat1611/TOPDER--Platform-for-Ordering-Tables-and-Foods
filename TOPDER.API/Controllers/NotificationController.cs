using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IHubContext<AppHub> _signalRHub;

        public NotificationController(INotificationService notificationService, IHubContext<AppHub> signalRHub)
        {
            _notificationService = notificationService;
            _signalRHub = signalRHub;
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Thêm mới thông báo")]
        public async Task<IActionResult> Create([FromBody] NotificationDto notificationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var notification = await _notificationService.AddAsync(notificationDto);
            if (notification != null)
            {

                //await _signalRHub.Clients.User(notification.Uid.ToString())
                //    .SendAsync("CreateNotification", notification.NotificationId, notification);

                await _signalRHub.Clients.All
                    .SendAsync("CreateNotification", notification.Uid, notification);

                return Ok(notification);
            }
            return BadRequest("Thêm thông báo thất bại.");
        }

        [HttpGet("GetNotification/{userId}/{notificationId}")]
        [SwaggerOperation(Summary = "Lấy thông tin chi tiết thông báo")]
        public async Task<IActionResult> GetById(int userId,int notificationId)
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

        [HttpGet("GetNotificationList")]
        [SwaggerOperation(Summary = "Lấy danh sách thông báo với phân trang")]
        public async Task<IActionResult> GetPaged(int pageNumber, int pageSize, [FromQuery] int userId)
        {
            var notifications = await _notificationService.GetPagingAsync(pageNumber, pageSize, userId);
            return Ok(notifications);
        }

        [HttpPut("IsRead/{id}")]
        [SwaggerOperation(Summary = "Đánh dấu thông báo là đã đọc")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _notificationService.IsReadAsync(id);
            if (result)
            {
                return Ok("Thông báo đã được đánh dấu là đã đọc.");
            }
            return NotFound("Không tìm thấy thông báo hoặc thông báo đã được đánh dấu là đã đọc.");
        }

        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập nhật dấu thông báo là đã đọc")]
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

        [HttpDelete("Delete/{userId}/{notificationId}")]
        [SwaggerOperation(Summary = "Xóa thông báo")]
        public async Task<IActionResult> Delete(int userId,int notificationId)
        {
            var result = await _notificationService.RemoveAsync(notificationId, userId);
            if (result)
            {
                return Ok("Xóa thông báo thành công.");
            }
            return NotFound("Không tìm thấy thông báo hoặc thông báo không thuộc về user.");
        }

    }
}
