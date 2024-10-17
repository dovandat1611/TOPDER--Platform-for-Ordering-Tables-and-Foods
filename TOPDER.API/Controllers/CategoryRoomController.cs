using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.Dtos.CategoryRoom;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryRoomController : ControllerBase
    {
        private readonly ICategoryRoomService _categoryRoomService;

        public CategoryRoomController(ICategoryRoomService categoryRoomService)
        {
            _categoryRoomService = categoryRoomService;
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo Category Room: Restaurant")]
        public async Task<IActionResult> CreateCategoryRoom([FromBody] CategoryRoomDto categoryRoomDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryRoomService.AddAsync(categoryRoomDto);
            if (result)
            {
                return Ok("Tạo Category Room thành công.");
            }

            return BadRequest("Tạo Category Room thất bại.");
        }

        [HttpGet("GetAllCategoryRooms/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy ra tất cả danh sách Category Room để chọn UPDATE và ADD: Restaurant")]
        public async Task<IActionResult> GetAllCategoryRooms(int restaurantId)
        {
            var result = await _categoryRoomService.GetAllCategoryRoomAsync(restaurantId);
            return Ok(result);
        }

        [HttpGet("GetCategoryRoom/{restaurantId}/{categoryRoomId}")]
        [SwaggerOperation(Summary = "Lấy ra một Category Room để Update: Restaurant")]
        public async Task<IActionResult> GetCategoryRoom(int restaurantId, int categoryRoomId)
        {
            try
            {
                var categoryRoom = await _categoryRoomService.GetItemAsync(categoryRoomId, restaurantId);
                return Ok(categoryRoom);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Category Room với ID {categoryRoomId} không tồn tại.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid($"Category Room với ID {categoryRoomId} không thuộc về nhà hàng với ID {restaurantId}.");
            }
        }

        [HttpGet("GetCategoryRoomList/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy danh sách Category Room của Nhà Hàng (có thể Search theo Name): Restaurant")]
        public async Task<IActionResult> GetCategoryRoomPaging(
            int restaurantId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? categoryRoomName = null)
        {
            if (restaurantId <= 0)
            {
                return BadRequest(new { message = "ID nhà hàng không hợp lệ." });
            }

            try
            {
                var result = await _categoryRoomService.ListPagingAsync(pageNumber, pageSize, restaurantId, categoryRoomName);

                var response = new PaginatedResponseDto<CategoryRoomDto>(
                    result,
                    result.PageIndex,
                    result.TotalPages,
                    result.HasPreviousPage,
                    result.HasNextPage
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Đã xảy ra lỗi trong quá trình xử lý yêu cầu của bạn: {ex.Message}" });
            }
        }


        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập nhật Category Room của Nhà Hàng: Restaurant")]
        public async Task<IActionResult> UpdateCategoryRoom([FromBody] CategoryRoomDto categoryRoomDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryRoomService.UpdateAsync(categoryRoomDto);
            if (result)
            {
                return Ok($"Cập nhật Category Room với ID {categoryRoomDto.CategoryRoomId} thành công.");
            }
            return NotFound($"Category Room với ID {categoryRoomDto.CategoryRoomId} không tồn tại.");
        }

        [HttpDelete("Delete/{categoryRoomId}")]
        [SwaggerOperation(Summary = "Xóa Category Room của Nhà Hàng: Restaurant")]
        public async Task<IActionResult> Remove(int categoryRoomId)
        {
            if (categoryRoomId <= 0)
            {
                return BadRequest(new { message = "ID không hợp lệ." });
            }

            try
            {
                var result = await _categoryRoomService.RemoveAsync(categoryRoomId);
                if (result)
                {
                    return Ok(new { message = "Xóa CategoryRoom thành công." });
                }
                return NotFound(new { message = "CategoryRoom không tìm thấy." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Đã xảy ra lỗi trong quá trình xử lý yêu cầu: {ex.Message}" });
            }
        }

    }
}
