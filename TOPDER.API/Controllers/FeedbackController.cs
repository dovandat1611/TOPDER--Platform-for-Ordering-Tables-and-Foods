using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection.Metadata;
using TOPDER.Repository.Entities;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.Dtos.FeedbackReply;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IHubContext<AppHub> _signalRHub;
        private readonly INotificationService _notificationService;
        private readonly IFeedbackReplyService _feedbackReplyService;



        public FeedbackController(IFeedbackService feedbackService, IHubContext<AppHub> signalRHub,
            INotificationService notificationService, IFeedbackReplyService feedbackReplyService)
        {
            _feedbackService = feedbackService;
            _signalRHub = signalRHub;
            _notificationService = notificationService;
            _feedbackReplyService = feedbackReplyService;
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo Feedback: Customer")]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackDto feedbackDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _feedbackService.AddAsync(feedbackDto);
            if (result != null)
            {
                NotificationDto notificationDto = new NotificationDto()
                {
                    NotificationId = 0,
                    Uid = result.RestaurantId ?? 0,
                    CreatedAt = DateTime.Now,
                    Content = Notification_Content.ADD_FEEDBACK(),
                    Type = Notification_Type.ADD_FEEDBACK,
                    IsRead = false,
                };

                var notification = await _notificationService.AddAsync(notificationDto);

                if (notification != null)
                {
                    List<NotificationDto> notifications = new List<NotificationDto> { notification };
                    await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                }

                return Ok(result);
            }
            return BadRequest("Failed to create feedback.");
        }


        [HttpPost("CreateFeedbackReply")]
        [SwaggerOperation(Summary = "Tạo FeedbackReply: Restaurant")]
        public async Task<IActionResult> AddFeedback([FromBody] CreateFeedbackReplyDto feedbackReplyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _feedbackReplyService.AddAsync(feedbackReplyDto);
            if (result != null)
            {
                NotificationDto notificationDto = new NotificationDto()
                {
                    NotificationId = 0,
                    Uid = result.CustomerId ?? 0,
                    CreatedAt = DateTime.Now,
                    Content = Notification_Content.ADD_FEEDBACKREPLY(),
                    Type = Notification_Type.ADD_FEEDBACK,
                    IsRead = false,
                };

                var notification = await _notificationService.AddAsync(notificationDto);

                if (notification != null)
                {
                    List<NotificationDto> notifications = new List<NotificationDto> { notification };
                    await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                }

                return Ok("Tạo thành công FeedbackReply");
            }
            return BadRequest("Failed to create feedbackReply.");
        }


        [HttpGet("GetFeedback/{orderId}")]
        public async Task<IActionResult> GetFeedback(int orderId)
        {
            try
            {
                var feedbackDto = await _feedbackService.GetFeedbackAsync(orderId);

                if (feedbackDto == null)
                {
                    return NotFound($"Feedback for Order ID {orderId} not found.");
                }

                return Ok(feedbackDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("GetFeedbacksHistory")]
        [SwaggerOperation(Summary = "Lấy ra cách feedback mà khách hàng từng tạo: Customer")]
        public async Task<IActionResult> GetHistoryCustomerPaging([FromQuery] int customerId)
        {
            var result = await _feedbackService.GetHistoryCustomerPagingAsync(customerId);
            return Ok(result);
        }


        [HttpPut("Invisible/{feedbackId}")]
        [SwaggerOperation(Summary = "Ẩn/Xóa Feedback: Customer | Admin")]
        public async Task<IActionResult> SetInvisible(int feedbackId)
        {
            var result = await _feedbackService.InvisibleAsync(feedbackId);
            if (result != null)
            {
                NotificationDto notificationResDto = new NotificationDto()
                {
                    NotificationId = 0,
                    Uid = result.RestaurantId ?? 0,
                    CreatedAt = DateTime.Now,
                    Content = Notification_Content.REMOVE_FEEDBACK_RESTAURANT(),
                    Type = Notification_Type.ADD_FEEDBACK,
                    IsRead = false,
                };

                NotificationDto notificationCusDto = new NotificationDto()
                {
                    NotificationId = 0,
                    Uid = result.CustomerId ?? 0,
                    CreatedAt = DateTime.Now,
                    Content = Notification_Content.REMOVE_FEEDBACK_CUSTOMER(),
                    Type = Notification_Type.ADD_FEEDBACK,
                    IsRead = false,
                };

                var notificationRes = await _notificationService.AddAsync(notificationResDto);
                var notificationCus = await _notificationService.AddAsync(notificationCusDto);


                if (notificationRes != null && notificationCus != null)
                {
                    List<NotificationDto> notifications = new List<NotificationDto> { notificationRes, notificationCus };
                    await _signalRHub.Clients.All.SendAsync("CreateNotification", notifications);
                }

                return Ok($"Ẩn/Xóa Feedback thành công.");
            }
            return NotFound($"Feedback with ID {feedbackId} not found.");
        }

        [HttpPut("InvisibleFeedbackReply/{replyId}")]
        [SwaggerOperation(Summary = "Ẩn/Xóa FeedbackReply: Restaurant")]
        public async Task<IActionResult> SetInvisibleFeedbackReply(int replyId)
        {
            var result = await _feedbackReplyService.InvisibleAsync(replyId);
            if (result)
            {
                return Ok($"Ẩn/Xóa FeedbackReply thành công.");
            }
            return NotFound($"Feedback with ID {replyId} not found.");
        }

        //[HttpPut("Update")]
        //[SwaggerOperation(Summary = "Cập nhật Feedback: Customer")]
        //public async Task<IActionResult> UpdateFeedback([FromBody] FeedbackDto feedbackDto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var result = await _feedbackService.UpdateAsync(feedbackDto);
        //    if (result)
        //    {
        //        return Ok("Feedback updated successfully.");
        //    }
        //    return NotFound("Feedback not found.");
        //}

        [HttpGet("GetFeedbackForRestaurantDetail/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy ra danh sách Feedback của nhà hàng đó trong CHI TIẾT NHÀ HÀNG: Customer")]
        public async Task<IActionResult> GetCustomerFeedbacks(int restaurantId)
        {
            var result = await _feedbackService.ListCustomerPagingAsync(restaurantId);
            return Ok(result);
        }

        [HttpGet("GetFeedbackListForAdmin")]
        [SwaggerOperation(Summary = "Lấy ra danh sách Feedback của toàn hệ thống: Admin")]
        public async Task<IActionResult> GetAdminFeedbacks()
        {
            var result = await _feedbackService.ListAdminPagingAsync();
            return Ok(result);
        }

        [HttpGet("GetFeedbackListForRestaurant/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy ra danh sách Feedback của nhà hàng: Restaurant")]
        public async Task<IActionResult> GetRestaurantFeedbacks(int restaurantId)
        {
            var result = await _feedbackService.ListRestaurantPagingAsync(restaurantId);
            return Ok(result);
        }

    }
}
