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

        [HttpPost("create")]
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
        public async Task<IActionResult> GetCategoryRoomPaging(int pageNumber, int pageSize, int restaurantId)
        {
            var result = await _categoryRoomService.GetPagingAsync(pageNumber, pageSize, restaurantId);
            
            var response = new PaginatedResponseDto<CategoryRoomDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCategoryRoom(int pageNumber, int pageSize, int restaurantId, string categoryRoomName)
        {
            var result = await _categoryRoomService.SearchPagingAsync(pageNumber, pageSize, restaurantId, categoryRoomName);
           
            var response = new PaginatedResponseDto<CategoryRoomDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpPut("update")]
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
        public async Task<IActionResult> DeleteCategoryRoom(int id)
        {
            var result = await _categoryRoomService.RemoveAsync(id);
            if (result)
            {
                return Ok($"Xóa Category Room với ID {id} thành công.");
            }
            return NotFound($"Category Room với ID {id} không tồn tại.");
        }
    }
}
