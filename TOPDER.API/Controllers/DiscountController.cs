using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public async Task<ActionResult> AddDiscount([FromBody] DiscountDto discountDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _discountService.AddAsync(discountDto);
            if (result)
            {
                return Ok("Tạo Discount thành công."); 
            }
            return BadRequest("Tạo Discount thất bại."); 
        }


        [HttpGet("available/{restaurantId}")]
        public async Task<ActionResult> GetAvailableDiscounts(int pageNumber, int pageSize, int restaurantId)
        {
            var result = await _discountService.GetAvailableDiscountsAsync(pageNumber, pageSize, restaurantId);
            var response = new PaginatedResponseDto<DiscountDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );
            return Ok(response);
        }

        [HttpGet("{restaurantId}/{id}")]
        public async Task<ActionResult> GetItem(int id, int restaurantId)
        {
            try
            {
                var discount = await _discountService.GetItemAsync(id, restaurantId);
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

 /*       [HttpGet("list/{restaurantId}")]
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
        }*/

 /*    
        [HttpPut]
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
        }*/
    }
}
