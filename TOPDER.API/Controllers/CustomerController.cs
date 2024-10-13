using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Dtos.Customer;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("Profile/{uid}")]
        public async Task<IActionResult> GetProfile(int uid)
        {
            var profile = await _customerService.Profile(uid);

            if (profile == null)
                return NotFound(new { Message = "Không tìm thấy khách hàng." });

            return Ok(profile);
        }

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] CustomerProfileDto customerProfile)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Dữ liệu không hợp lệ." });

            var result = await _customerService.UpdateProfile(customerProfile);

            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "Cập nhật thông tin không thành công." });

            return Ok(new { Message = "Cập nhật thông tin thành công." });
        }

    }
}
