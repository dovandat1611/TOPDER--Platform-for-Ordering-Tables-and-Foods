using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Swashbuckle.AspNetCore.Annotations;
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


        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo món ăn ở nhà hàng: Restaurant")]
        public async Task<IActionResult> Create([FromBody] MenuDto menuDto)
        {
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

        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập Nhật món ăn ở nhà hàng: Restaurant")]
        public async Task<IActionResult> Update([FromBody] MenuDto menuDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

        [HttpPost("CreateByExcel")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Tạo một list món ăn thông qua file Excel: Restaurant")]
        public async Task<IActionResult> AddRangeFromExcel([FromForm] CreateExcelMenuDto createExcelMenuDto)
        {
            var (isSuccess, message) = await _menuService.AddRangeExcelAsync(createExcelMenuDto);

            if (!isSuccess)
            {
                return BadRequest(new { Error = message });
            }

            return Ok(new { Message = "Menu items added successfully from Excel." });
        }


        [HttpPut("Invisible/{restaurantId}/{menuId}")]
        [SwaggerOperation(Summary = "Xóa/Ẩn món ăn : Restaurant")]
        public async Task<IActionResult> GetInvisible(int restaurantId, int menuId)
        {
            var result = await _menuService.InvisibleAsync(menuId, restaurantId);
            if (!result)
            {
                return NotFound("Món ăn không được tìm thấy, không thuộc về nhà hàng đã chỉ định, hoặc đang được sử dụng trong một đơn hàng.");
            }
            return Ok("Xóa/Ẩn Món ăn đã thành công.");
        }

        [HttpGet("GetMenuListForRestaurant/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy món ăn của nhà hàng: Restaurant")]
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

        [HttpGet("GetMenuListForCustomer/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy món ăn của nhà hàng đó (món ăn phải active): Customer")]
        public async Task<IActionResult> GetCustomerMenuList(int restaurantId)
        {
            var result = await _menuService.ListMenuCustomerByCategoryMenuAsync(restaurantId);
            return Ok(result);
        }


        [HttpPut("IsActive/{menuId}")]
        public async Task<IActionResult> UpdateMenuStatus(int menuId, [FromQuery] string status)
        {
            var isUpdated = await _menuService.IsActiveAsync(menuId, status);

            if (isUpdated)
            {
                return Ok(new { message = "Menu status updated successfully." });
            }

            return BadRequest(new { message = "Failed to update menu status. Please check the menu ID and status value." });
        }

    }
}
