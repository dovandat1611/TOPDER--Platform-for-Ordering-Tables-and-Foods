using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.Dtos.CategoryRoom;
using TOPDER.Service.IServices;

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

    /*    [HttpPost]
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
        }*/

        [HttpGet("{restaurantId}/{id}")]
        public async Task<IActionResult> GetCategoryRoom(int restaurantId, int id)
        {
            try
            {
                var categoryRoom = await _categoryRoomService.GetItemAsync(id, restaurantId);
                return Ok(categoryRoom);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Category Room với ID {id} không tồn tại.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid($"Category Room với ID {id} không thuộc về nhà hàng với ID {restaurantId}.");
            }
        }

        [HttpGet("list/{restaurantId}")]
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


        [HttpPut]
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "ID không hợp lệ." });
            }

            try
            {
                var result = await _categoryRoomService.RemoveAsync(id);
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
