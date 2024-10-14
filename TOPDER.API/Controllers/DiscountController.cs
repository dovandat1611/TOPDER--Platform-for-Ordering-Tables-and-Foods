using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo Discount : Restaurant")]
        public async Task<IActionResult> AddDiscount([FromBody] DiscountDto discountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                bool result = await _discountService.AddAsync(discountDto);

                if (result)
                {
                    return Ok(new { Message = "Tạo khuyến mãi thành công." });
                }
                return BadRequest(new { Message = "Tạo khuyến mãi thất bại." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi tạo khuyến mãi.", Details = ex.Message });
            }
        }

        [HttpGet("GetAvailableDiscount/{restaurantId}/{customerId}/{totalPrice}")]
        [SwaggerOperation(Summary = "Khi click vào đặt bàn thì sẽ hiện ra những mã giảm giá tương ứng với đơn hàng dựa theo Customer, Restaurant, Giá trị đơn hàng: Customer")]
        public async Task<ActionResult> GetAvailableDiscounts(int restaurantId, int customerId, decimal totalPrice)
        {
            var result = await _discountService.GetAvailableDiscountsAsync(restaurantId, customerId, totalPrice);
            return Ok(result);
        }

        [HttpGet("GetDiscount/{restaurantId}/{discountId}")]
        [SwaggerOperation(Summary = "Lấy ra một Discount để Update: Restaurant")]
        public async Task<ActionResult> GetItem(int restaurantId, int discountId)
        {
            try
            {
                var discount = await _discountService.GetItemAsync(discountId, restaurantId);
                return Ok(discount);
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

        [HttpGet("GetDiscountList/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy danh sách Discount của nhà hàng: Restaurant")]
        public async Task<ActionResult> GetRestaurantPaging(int pageNumber, int pageSize, int restaurantId)
        {
            var result = await _discountService.GetRestaurantPagingAsync(pageNumber, pageSize, restaurantId);

            var response = new PaginatedResponseDto<DiscountDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );
            return Ok(response);
        }

        [HttpDelete("Delete/{restaurantId}/{discountId}")]
        [SwaggerOperation(Summary = "Xóa Discount của nhà hàng: Restaurant")]
        public async Task<ActionResult> RemoveDiscount(int restaurantId, int discountId)
        {
            var result = await _discountService.RemoveAsync(discountId, restaurantId);
            if (result)
            {
                return Ok("Xóa Discount thành công.");
            }
            return NotFound("Giảm giá không tồn tại hoặc không thuộc về nhà hàng.");
        }

        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập Nhật Discount của nhà hàng: Restaurant")]
        public async Task<ActionResult> UpdateDiscount([FromBody] DiscountDto discountDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _discountService.UpdateAsync(discountDto);
            if (result)
            {
                return Ok("Cập Nhật Discount thành công."); 
            }
            return NotFound("Giảm giá không tồn tại hoặc không thuộc về nhà hàng.");
        }

    }
}
