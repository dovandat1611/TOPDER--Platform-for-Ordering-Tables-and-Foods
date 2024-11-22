using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryRestaurantController : ControllerBase
    {
        private readonly ICategoryRestaurantService _categoryRestaurantService;

        public CategoryRestaurantController(ICategoryRestaurantService categoryRestaurantService)
        {
            _categoryRestaurantService = categoryRestaurantService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryRestaurantDto categoryRestaurantDto)
        {
            var result = await _categoryRestaurantService.AddAsync(categoryRestaurantDto);
            if (result)
            {
                return Ok(new { message = "Tạo danh mục nhà hàng thành công." });
            }
            return BadRequest(new { message = "Lỗi khi tạo danh mục nhà hàng." }); 
        }


     /*   [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _categoryRestaurantService.UpdateItemAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
*/

        [HttpGet("list")]
        public async Task<IActionResult> ListPaging(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? categoryRestaurantName = null)
        {
            try
            {
                var result = await _categoryRestaurantService.ListPagingAsync(pageNumber, pageSize, categoryRestaurantName);

                var response = new PaginatedResponseDto<CategoryRestaurantDto>(
                    result,
                    result.PageIndex,
                    result.TotalPages,
                    result.HasPreviousPage,
                    result.HasNextPage
                );

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryRestaurantDto categoryRestaurantDto)
        {
            if (id != categoryRestaurantDto.CategoryRestaurantId)
            {
                return BadRequest(new { message = "ID không khớp." }); 
            }

            var result = await _categoryRestaurantService.UpdateAsync(categoryRestaurantDto);
            if (result)
            {
                return Ok(new { message = "Cập nhật danh mục nhà hàng thành công." }); 
            }

            return NotFound(new { message = "Không tìm thấy danh mục nhà hàng." }); 
        }


        [HttpGet("exists")]
        public async Task<IActionResult> CategoryExist()
        {
            try
            {
                var categories = await _categoryRestaurantService.CategoryExistAsync();

                if (categories == null || !categories.Any())
                {
                    return NotFound("No categories found that are associated with any restaurant.");
                }

                return Ok(categories);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }

    }
}
