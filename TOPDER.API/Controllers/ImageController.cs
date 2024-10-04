using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Service.Services;
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

        [HttpPost]
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


        // Lấy thông tin hình ảnh
        [HttpGet("{imageId}/{restaurantId}")]
        public async Task<IActionResult> GetImage(int imageId, int restaurantId)
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

        // Phân trang danh sách hình ảnh của nhà hàng
        [HttpGet("list/{restaurantId}")]
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

        // Cập nhật hình ảnh
        [HttpPut("{imageId}")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromBody] ImageDto imageDto)
        {
            if (imageDto == null || imageDto.ImageId != imageId)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            var result = await _imageService.UpdateAsync(imageDto);
            if (result)
            {
                return Ok("Cập nhật hình ảnh thành công.");
            }

            return NotFound($"Không tìm thấy hình ảnh với Id {imageId}.");
        }

        // Xóa hình ảnh
        [HttpDelete("{imageId}/restaurant/{restaurantId}")]
        public async Task<IActionResult> RemoveImage(int imageId, int restaurantId)
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
