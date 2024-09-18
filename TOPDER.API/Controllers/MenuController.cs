using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
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


        [HttpPost("Create")]
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

        [HttpPut("Update")]
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

    }
}
