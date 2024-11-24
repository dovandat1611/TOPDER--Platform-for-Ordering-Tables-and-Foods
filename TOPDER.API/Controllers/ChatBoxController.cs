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

    }
}
