using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;

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

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo Category Restaurant: Admin")]
        public async Task<IActionResult> Create([FromBody] CategoryRestaurantDto categoryRestaurantDto)
        {
            var result = await _categoryRestaurantService.AddAsync(categoryRestaurantDto);
            if (result)
            {
                return Ok(new { message = "Tạo danh mục nhà hàng thành công." });
            }
            return BadRequest(new { message = "Lỗi khi tạo danh mục nhà hàng." }); 
        }

        [HttpGet("GetAllCategoryRestaurants")]
        [SwaggerOperation(Summary = "Lấy ra tất cả danh sách Category Restaurant để chọn Register và Update cho Nhà Hàng: Restaurant")]
        public async Task<IActionResult> GetAllCategoryRestaurants()
        {
            var result = await _categoryRestaurantService.GetAllCategoryRestaurantAsync();
            return Ok(result);
        }


        [HttpGet("GetCategoryRestaurant/{categoryRestaurantId}")]
        [SwaggerOperation(Summary = "Lấy ra một Category Restaurant để Update: Admin")]
        public async Task<IActionResult> GetById(int categoryRestaurantId)
        {
            try
            {
                var result = await _categoryRestaurantService.UpdateItemAsync(categoryRestaurantId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpGet("GetCategoryRestaurantList")]
        [SwaggerOperation(Summary = "Lấy danh sách Category Restaurant (có thể Search theo Name): Admin")]
        public async Task<IActionResult> ListPaging(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? categoryRestaurantName = null)
        {
            try
            {
                var result = await _categoryRestaurantService.ListPagingAsync(pageNumber, pageSize, categoryRestaurantName);

                var response = new PaginatedResponseDto<CategoryRestaurantViewDto>(
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

        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập nhật Category Restaurant: Admin")]
        public async Task<IActionResult> Update([FromBody] CategoryRestaurantDto categoryRestaurantDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryRestaurantService.UpdateAsync(categoryRestaurantDto);
            if (result)
            {
                return Ok(new { message = "Cập nhật danh mục nhà hàng thành công." }); 
            }

            return NotFound(new { message = "Không tìm thấy danh mục nhà hàng." }); 
        }


        [HttpGet("GetExistingCategoriesRestaurant")]
        [SwaggerOperation(Summary = "Lấy ra những CategoryRestaurant có trong Restaurant (phục vụ cho việc Search): Customer")]
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

        [HttpDelete("Delete/{categoryRestaurantId}")]
        [SwaggerOperation(Summary = "Xóa Report: Admin")]
        public async Task<IActionResult> DeleteReport(int categoryRestaurantId)
        {
            var result = await _categoryRestaurantService.RemoveAsync(categoryRestaurantId);
            if (result)
                return Ok(new { Message = "Category Restaurant deleted successfully." });

            return NotFound(new { Message = $"Report with ID {categoryRestaurantId} not found." });
        }

    }
}
