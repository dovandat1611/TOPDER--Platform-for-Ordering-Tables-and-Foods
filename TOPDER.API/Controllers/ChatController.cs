using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Dtos.Chat;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateChat([FromBody] CreateorUpdateChatDto createChatDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _chatService.AddAsync(createChatDto);
            if (result)
            {
                return Ok("Tạo chat thành công.");
            }

            return BadRequest("Tạo chat thất bại.");
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetChat(int id)
        {
            try
            {
                var chat = await _chatService.GetItemAsync(id);
                return Ok(chat);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Chat với ID {id} không tồn tại.");
            }
        }

        [HttpGet("list/{chatBoxId}")]
        public async Task<IActionResult> GetChatList(int chatBoxId)
        {
            var chats = await _chatService.GetListAsync(chatBoxId);
            return Ok(chats);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var result = await _chatService.RemoveAsync(id);
            if (result)
            {
                return Ok($"Xóa chat với ID {id} thành công.");
            }
            return NotFound($"Chat với ID {id} không tồn tại.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateChat([FromBody] CreateorUpdateChatDto updateChatDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _chatService.UpdateAsync(updateChatDto);
            if (result)
            {
                return Ok("Cập nhật chat thành công.");
            }
            return NotFound($"Chat với ID {updateChatDto.ChatId} không tồn tại.");
        }
    }
}
