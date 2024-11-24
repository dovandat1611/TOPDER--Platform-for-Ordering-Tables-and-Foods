using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly ICloudinaryService _cloudinaryService;

        public MenuController(IMenuService menuService, ICloudinaryService cloudinaryService)
        {
            _menuService = menuService;
            _cloudinaryService = cloudinaryService;
        }


        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] MenuDto menuDto, IFormFile File)
        {
            if (File == null || File.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            var uploadResult = await _cloudinaryService.UploadImageAsync(File);
            if (uploadResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Image upload failed.");
            }

            menuDto.Image = uploadResult.SecureUrl?.ToString();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var addMenu = await _menuService.AddAsync(menuDto);
                return Ok(addMenu);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to create restaurant menu: {ex.Message}");
            }
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] MenuDto menuDto, IFormFile File)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (File != null && File.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(File);
                if (uploadResult == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Image upload failed.");
                }
                menuDto.Image = uploadResult.SecureUrl?.ToString();
            }

            try
            {
                var updatedMenu = await _menuService.UpdateAsync(menuDto);
                return Ok(updatedMenu);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to update restaurant menu: {ex.Message}");
            }
        }

      /*  [HttpPost("import-excel")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddRangeFromExcel([FromForm] CreateExcelMenuDto createExcelMenuDto)
        {
            var result = await _menuService.AddRangeExcelAsync(createExcelMenuDto);
            if (!result)
            {
                return StatusCode(500, "Failed to add menu items from Excel.");
            }

            return Ok("Menu items added successfully from Excel.");
        }

        [HttpDelete("remove/{restaurantId}/{id}")]
        public async Task<IActionResult> RemoveMenu(int restaurantId, int id)
        {
            var result = await _menuService.RemoveAsync(id, restaurantId);
            if (!result)
            {
                return NotFound("Món ăn không được tìm thấy, không thuộc về nhà hàng đã chỉ định, hoặc đang được sử dụng trong một đơn hàng.");
            }
            return Ok("Món ăn đã được xóa thành công.");
        }
*/
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetMenuList(
            int restaurantId,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromQuery] int? categoryMenuId = null,
            [FromQuery] string? menuName = null)
        {
            var result = await _menuService.ListRestaurantPagingAsync(pageNumber, pageSize, restaurantId, categoryMenuId, menuName);

            if (result == null || !result.Any())
            {
                return NotFound("Không tìm thấy món ăn nào cho nhà hàng được chỉ định.");
            }

            var response = new PaginatedResponseDto<MenuRestaurantDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpGet("customer/{restaurantId}")]
        public async Task<IActionResult> GetCustomerMenuList(
            int restaurantId,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize)
        {
            var result = await _menuService.ListCustomerPagingAsync(pageNumber, pageSize, restaurantId);

            if (result == null || !result.Any())
            {
                return NotFound("Không tìm thấy món ăn nào cho nhà hàng được chỉ định.");
            }

            var response = new PaginatedResponseDto<MenuCustomerDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }


    }
}
