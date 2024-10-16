using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Service.Services;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Dtos.Image;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        private readonly ICloudinaryService _cloudinaryService;

        public ImageController(IImageService imageService, ICloudinaryService cloudinaryService)
        {
            _imageService = imageService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("UploadFileToCloudinary")]
        [SwaggerOperation(Summary = "Tải 1 ảnh lên cloud và lấy vễ đường dẫn")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "File không hợp lệ." });
            }

            var uploadResult = await _cloudinaryService.UploadImageAsync(file);

            if (uploadResult == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Lỗi trong quá trình upload hình ảnh." });
            }

            var imageUrl = uploadResult.SecureUrl.ToString();

            return Ok(new { Message = "Tải lên hình ảnh thành công.", Url = imageUrl });
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Có tạo tải nhiều ảnh: Restaurant")]
        public async Task<IActionResult> AddImage(int restaurantId, List<IFormFile> files)
        {
            if (files == null || !files.Any())
            {
                return BadRequest("Không có file nào để upload.");
            }

            var uploadResults = await _cloudinaryService.UploadImagesAsync(files);

            // Lọc kết quả upload thành công
            var mediaUrls = uploadResults
                .Where(result => result.StatusCode == System.Net.HttpStatusCode.OK && result.SecureUrl != null)
                .Select(result => result.SecureUrl.ToString())
                .ToList();

            if (!mediaUrls.Any())
            {
                return BadRequest("Không có ảnh nào được tải lên thành công.");
            }

            List<ImageDto> ListImageDto = new List<ImageDto>();
            foreach (var mediaUrl in mediaUrls)
            {
                ImageDto imageDto = new ImageDto()
                {
                    ImageId = 0,
                    RestaurantId = restaurantId,
                    ImageUrl = mediaUrl,
                };
                ListImageDto.Add(imageDto);
            }

            var result = await _imageService.AddRangeAsync(ListImageDto);
            if (result)
            {
                return Ok(new { Message = "Thêm hình ảnh thành công.", Urls = mediaUrls });
            }

            return StatusCode(500, "Có lỗi xảy ra khi thêm hình ảnh.");
        }


        [HttpGet("GetImage/{restaurantId}/{imageId}")]
        [SwaggerOperation(Summary = "Lấy thông tin của ảnh phục vụ cho Update: Restaurant")]
        public async Task<IActionResult> GetImage(int restaurantId,int imageId)
        {
            try
            {
                var image = await _imageService.GetItemAsync(imageId, restaurantId);
                return Ok(image);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetImageList/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy danh sách hình ảnh của nhà hàng: Restaurant")]
        public async Task<IActionResult> GetPagingImages(int restaurantId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedImages = await _imageService.GetPagingAsync(pageNumber, pageSize, restaurantId);
                return Ok(paginatedImages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy dữ liệu: {ex.Message}");
            }
        }

        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập nhật hình ảnh của nhà hàng: Restaurant")]
        public async Task<IActionResult> UpdateImage([FromBody] ImageDto imageDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _imageService.UpdateAsync(imageDto);
            if (result)
            {
                return Ok("Cập nhật hình ảnh thành công.");
            }

            return NotFound($"Không tìm thấy hình ảnh với Id {imageDto.ImageId}.");
        }

        [HttpDelete("Delete/{restaurantId}/{imageId}")]
        [SwaggerOperation(Summary = "Xóa hình ảnh của nhà hàng: Restaurant")]
        public async Task<IActionResult> RemoveImage(int restaurantId, int imageId)
        {
            try
            {
                var result = await _imageService.RemoveAsync(imageId, restaurantId);
                if (result)
                {
                    return Ok($"Đã xóa hình ảnh với Id {imageId}.");
                }
                return NotFound($"Không tìm thấy hình ảnh với Id {imageId}.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
