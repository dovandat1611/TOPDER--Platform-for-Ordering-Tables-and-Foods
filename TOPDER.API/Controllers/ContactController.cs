using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.ChatBox;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly ISendMailService _sendMailService;

        public ContactController(IContactService contactService, ISendMailService sendMailService)
        {
            _contactService = contactService;
            _sendMailService = sendMailService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddContact([FromBody] ContactDto contactDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _contactService.AddAsync(contactDto);
            if (result)
            {
                await _sendMailService.SendEmailAsync(contactDto.Email, Email_Subject.CONTACT, EmailTemplates.Contact(contactDto.Name));
                return Ok($"Tạo liên hệ thành công.");
            }
            return BadRequest("Thêm liên hệ thất bại.");
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetPaging([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _contactService.GetPagingAsync(pageNumber, pageSize);

            var response = new PaginatedResponseDto<ContactDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var result = await _contactService.RemoveAsync(id);
            if (result)
            {
                return Ok($"Xóa liên hệ với ID {id} thành công.");
            }
            return NotFound($"Liên hệ với ID {id} không tồn tại.");
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string content, [FromQuery] string topic, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _contactService.SearchPagingAsync(pageNumber, pageSize, content, topic);

            var response = new PaginatedResponseDto<ContactDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }
    }
}
