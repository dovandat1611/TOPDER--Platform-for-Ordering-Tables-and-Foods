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
        private readonly CloudinaryService _cloudinaryService;

        public MenuController(IMenuService menuService, CloudinaryService cloudinaryService)
        {
            _menuService = menuService;
            _cloudinaryService = cloudinaryService;
        }


        [HttpPost("add")]
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

        [HttpPut("update")]
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

        [HttpPost("import-excel")]
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

        [HttpGet("restaurant-list")]
        public async Task<IActionResult> GetMenuList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] int restaurantId = 0)
        {
            var result = await _menuService.GetPagingAsync(pageNumber, pageSize, restaurantId);

            var response = new PaginatedResponseDto<MenuRestaurantDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpGet("restaurant-search")]
        public async Task<IActionResult> SearchMenus([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] int restaurantId = 0, [FromQuery] int categoryMenuId = 0, [FromQuery] string menuName = "")
        {
            var result = await _menuService.SearchPagingAsync(pageNumber, pageSize, restaurantId, categoryMenuId, menuName);
            var response = new PaginatedResponseDto<MenuRestaurantDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );
            return Ok(response);
        }

        [HttpDelete("remove/{id}/{restaurantId}")]
        public async Task<IActionResult> RemoveMenu(int id, int restaurantId)
        {
            var result = await _menuService.RemoveAsync(id, restaurantId);
            if (!result)
            {
                return NotFound("Menu item not found or does not belong to the specified restaurant.");
            }

            return Ok("Menu item removed successfully.");
        }


        [HttpGet("customer-list")]
        public async Task<IActionResult> GetCustomerMenuList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] int restaurantId = 0)
        {
            var result = await _menuService.GetCustomerPagingAsync(pageNumber, pageSize, restaurantId);

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
