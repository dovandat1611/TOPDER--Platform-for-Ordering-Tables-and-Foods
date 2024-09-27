using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryMenuController : ControllerBase
    {
        private readonly ICategoryMenuService _categoryMenuService;

        public CategoryMenuController(ICategoryMenuService categoryMenuService)
        {
            _categoryMenuService = categoryMenuService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategoryMenu([FromBody] CreateCategoryMenuDto categoryMenuDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryMenuService.AddAsync(categoryMenuDto);
            if (result)
            {
                return Ok("Tạo Category Menu thành công.");
            }

            return BadRequest("Tạo Category Menu thất bại.");
        }

        [HttpGet("{id}/{restaurantId}")]
        public async Task<IActionResult> GetCategoryMenu(int id, int restaurantId)
        {
            try
            {
                var categoryMenu = await _categoryMenuService.GetItemAsync(id, restaurantId);
                return Ok(categoryMenu);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Category Menu với ID {id} không tồn tại.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid($"Category Menu với ID {id} không thuộc về nhà hàng với ID {restaurantId}.");
            }
        }

        [HttpGet("paging/{restaurantId}")]
        public async Task<IActionResult> GetCategoryMenuPaging(int pageNumber, int pageSize, int restaurantId)
        {
            var result = await _categoryMenuService.GetPagingAsync(pageNumber, pageSize, restaurantId);

            var response = new PaginatedResponseDto<CategoryMenuDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCategoryMenu(int pageNumber, int pageSize, int restaurantId, string categoryMenuName)
        {
            var result = await _categoryMenuService.SearchPagingAsync(pageNumber, pageSize, restaurantId, categoryMenuName);
            
            var response = new PaginatedResponseDto<CategoryMenuDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCategoryMenu([FromBody] CategoryMenuDto categoryMenuDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryMenuService.UpdateAsync(categoryMenuDto);
            if (result)
            {
                return Ok($"Cập nhật Category Menu với ID {categoryMenuDto.CategoryMenuId} thành công.");
            }
            return NotFound($"Category Menu với ID {categoryMenuDto.CategoryMenuId} không tồn tại.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryMenu(int id)
        {
            var result = await _categoryMenuService.RemoveAsync(id);
            if (result)
            {
                return Ok($"Xóa Category Menu với ID {id} thành công.");
            }
            return NotFound($"Category Menu với ID {id} không tồn tại.");
        }
    }
}
