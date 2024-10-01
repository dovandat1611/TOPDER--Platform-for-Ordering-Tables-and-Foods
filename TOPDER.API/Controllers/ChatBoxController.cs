using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChatBox(int id)
        {
            try
            {
                var chatBox = await _chatBoxService.GetItemAsync(id);
                return Ok(chatBox);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Chat Box với ID {id} không tồn tại.");
            }
        }

        [HttpGet("list/{id}")]
        public async Task<IActionResult> GetChatBoxPaging(int pageNumber, int pageSize, int id)
        {
            var result = await _chatBoxService.GetPagingAsync(pageNumber, pageSize, id);

            var response = new PaginatedResponseDto<ChatBoxDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpDelete("{id}")]
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
