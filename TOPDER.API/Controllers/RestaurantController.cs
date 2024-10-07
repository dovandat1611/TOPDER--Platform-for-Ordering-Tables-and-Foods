using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ISendMailService _sendMailService;

        public RestaurantController(IRestaurantService restaurantService, ICloudinaryService cloudinaryService, ISendMailService sendMailService)
        {
            _restaurantService = restaurantService;
            _cloudinaryService = cloudinaryService;
            _sendMailService = sendMailService;
        }

        [HttpGet("customer/service")]
        public async Task<IActionResult> GetItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null, [FromQuery] string? address = null,
            [FromQuery] string? location = null, [FromQuery] int? restaurantCategory = null,
            [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? maxCapacity = null)
        {
            var result = await _restaurantService.GetItemsAsync(pageNumber, pageSize, name, address, location,
                restaurantCategory, minPrice, maxPrice, maxCapacity);

            var response = new PaginatedResponseDto<RestaurantDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );
            return Ok(response);
        }

        [HttpGet("customer/home")]
        public async Task<IActionResult> GetHomeItems()
        {
            var result = await _restaurantService.GetHomeItemsAsync();
            return Ok(result);
        }

        [HttpGet("customer/detail/{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            try
            {
                var result = await _restaurantService.GetItemAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("Restaurant/UpdateDiscountAndFee/{restaurantId}")]
        public async Task<IActionResult> UpdateDiscountAndFee(
            int restaurantId,
            [FromQuery] decimal? discountPrice,
            [FromQuery] decimal? firstFeePercent,
            [FromQuery] decimal? returningFeePercent,
            [FromQuery] decimal? cancellationFeePercent)
        {
            var result = await _restaurantService.UpdateDiscountAndFeeAsync(
                restaurantId,
                discountPrice,
                firstFeePercent,
                returningFeePercent,
                cancellationFeePercent
            );

            if (result)
            {
                return Ok(new { message = "Cập nhật chiết khấu và phí thành công." });
            }
            else
            {
                return BadRequest(new { message = "Không có thay đổi nào được thực hiện." });
            }
        }

        [HttpGet("Restaurant/GetDiscountAndFee/{restaurantId}")]
        public async Task<IActionResult> GetDiscountAndFee(int restaurantId)
        {
            var discountAndFee = await _restaurantService.GetDiscountAndFeeAsync(restaurantId);

            if (discountAndFee == null)
            {
                return NotFound(new { message = "Nhà hàng không tồn tại." });
            }

            return Ok(discountAndFee);
        }

        [HttpGet("Restaurant/GetDescription/{restaurantId}")]
        public async Task<IActionResult> GetDescription(int restaurantId)
        {
            var description = await _restaurantService.GetDescriptionAsync(restaurantId);

            if (description == null)
            {
                return NotFound(new { message = "Nhà hàng không tồn tại." });
            }

            return Ok(description);
        }

        [HttpPut("Restaurant/UpdateDescription/{restaurantId}")]
        public async Task<IActionResult> UpdateDescription(
            int restaurantId,
            [FromQuery] string? description,
            [FromQuery] string? subDescription)
        {
            var result = await _restaurantService.UpdateDescriptionAsync(restaurantId, description, subDescription);

            if (result)
            {
                return Ok(new { message = "Cập nhật mô tả thành công." });
            }
            else
            {
                return BadRequest(new { message = "Không có thay đổi nào được thực hiện hoặc nhà hàng không tồn tại." });
            }
        }

        [HttpPut("booking-enabled/{id}")]
        public async Task<IActionResult> UpdateBookingEnabled(int id, [FromBody] bool isEnabledBooking)
        {
            try
            {
                bool result = await _restaurantService.IsEnabledBookingAsync(id, isEnabledBooking);
                if (result)
                {
                    return Ok(new { Message = "Trạng thái đặt bàn đã được cập nhật thành công." });
                }
                return Ok(new { Message = "Trạng thái đặt bàn không có gì thay đổi." }); // Trả về 204 No Content nếu không có thay đổi
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Đã xảy ra lỗi: {ex.Message}" }); // Trả về 400 Bad Request nếu có lỗi
            }
        }

    }
}
