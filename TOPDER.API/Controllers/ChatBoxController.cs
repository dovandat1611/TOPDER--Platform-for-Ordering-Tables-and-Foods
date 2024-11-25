using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.CategoryRoom;
using TOPDER.Service.Dtos.ChatBox;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBoxController : ControllerBase
    {
        private readonly IChatBoxService _chatBoxService;

        public ChatBoxController(IChatBoxService chatBoxService)
        {
            _chatBoxService = chatBoxService;
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo ChatBox để nhắn tin với Nhà Hàng: Customer")]
        public async Task<IActionResult> CreateChatBox([FromBody] CreateChatBoxDto chatBoxDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _chatBoxService.AddAsync(chatBoxDto);
            if (result)
            {
                return Ok("Tạo Chat Box thành công.");
            }

            return BadRequest("Tạo Chat Box thất bại.");
        }

        //[HttpGet("GetChatBox/{chatBoxId}")]
        //[SwaggerOperation(Summary = "Lấy ra một ChatBox: Restaurant | Customer")]
        //public async Task<IActionResult> GetChatBox(int chatBoxId)
        //{
        //    try
        //    {
        //        var chatBox = await _chatBoxService.GetItemAsync(chatBoxId);
        //        return Ok(chatBox);
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound($"Chat Box với ID {chatBoxId} không tồn tại.");
        //    }
        //}


        [HttpGet("CheckExist/{customerId}/{restaurantId}")]
        [SwaggerOperation(Summary = "Check xem ChatBox đã tồn tại chưa: Restaurant | Customer")]
        public async Task<IActionResult> GetChatBox(int customerId, int restaurantId)
        {
                var chatBox = await _chatBoxService.CheckExistAsync(customerId, restaurantId);
                return Ok(chatBox);
        }

        [HttpGet("GetChatBoxList/{userId}")]
        [SwaggerOperation(Summary = "Lấy danh sách ChatBox của User: Restaurant | Customer")]
        public async Task<IActionResult> GetChatBoxPaging(int userId)
        {
            var result = await _chatBoxService.GetChatListAsync(userId);

            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Xóa ChatBox (bao gồm cả nội dung Chat): Restaurant | Customer")]
        public async Task<IActionResult> DeleteChatBox(int id)
        {
            var result = await _chatBoxService.RemoveAsync(id);
            if (result)
            {
                return Ok($"Xóa Chat Box với ID {id} thành công.");
            }
            return NotFound($"Chat Box với ID {id} không tồn tại.");
        }

        [HttpPost("IsRead")]
        public async Task<IActionResult> MarkAsRead([FromQuery] int uid, [FromQuery] int chatboxId)
        {
            var result = await _chatBoxService.IsReadAsync(uid, chatboxId);
            if (!result)
            {
                return NotFound(new { message = "Chatbox không tồn tại hoặc không thể cập nhật." });
            }

            return Ok(new { message = "Đánh dấu đã đọc thành công." });
        }

        [HttpPost("IsReadAll")]
        public async Task<IActionResult> MarkAllAsRead([FromQuery] int uid)
        {
            var result = await _chatBoxService.IsReadAllAsync(uid);
            if (!result)
            {
                return NotFound(new { message = "Không có chatbox nào để cập nhật." });
            }

            return Ok(new { message = "Tất cả chatbox đã được đánh dấu là đã đọc." });
        }

    }
}
