using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Dtos.Customer;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ICloudinaryService _cloudinaryService;

        public CustomerController(ICustomerService customerService, ICloudinaryService cloudinaryService)
        {
            _customerService = customerService;
            _cloudinaryService = cloudinaryService;
        }


        [HttpPut("UpdateProfile")]
        [SwaggerOperation(Summary = "cập nhật lại thông tin profile : Customer")]
        public async Task<IActionResult> UpdateProfile([FromForm] CustomerProfileDto customerProfile)
        {   
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Dữ liệu không hợp lệ." });

            if(customerProfile.ImageFile != null && customerProfile.ImageFile.Length > 0)
            {
                var uploadResults = await _cloudinaryService.UploadImageAsync(customerProfile.ImageFile);

                if(uploadResults != null)
                {
                    customerProfile.Image = uploadResults.SecureUrl.ToString();
                }
            }

            var result = await _customerService.UpdateProfile(customerProfile);

            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "Cập nhật thông tin không thành công." });

            return Ok(new { Message = "Cập nhật thông tin thành công." });
        }

    }
}
