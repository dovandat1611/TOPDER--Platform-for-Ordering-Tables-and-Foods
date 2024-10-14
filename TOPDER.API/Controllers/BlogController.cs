using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }


        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo Blog: Admin")]
        public async Task<IActionResult> CreateBlog([FromBody] CreateBlogModel createBlogModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _blogService.AddAsync(createBlogModel);
            if (!result)
            {
                return StatusCode(500, "Có lỗi xảy ra khi thêm bài viết.");
            }
            return Ok($"Tạo Blog thành công.");
        }

        [HttpGet("GetBlogForUpdate/{blogId}")]
        [SwaggerOperation(Summary = "Lấy thông tin của blog để cập nhật: Admin")]
        public async Task<IActionResult> GetUpdateItemAsync(int blogId)
        {
            try
            {
                var updateBlogModel = await _blogService.GetUpdateItemAsync(blogId);
                return Ok(updateBlogModel);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}"); 
            }
        }

        [HttpGet("GetDetailBlog/{id}")]
        [SwaggerOperation(Summary = "Xem chi tiết Blog: Customer")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            try
            {
                var blogDetail = await _blogService.GetBlogByIdAsync(id);
                return Ok(blogDetail);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập nhật Blog: Admin")]
        public async Task<IActionResult> UpdateBlog([FromBody] UpdateBlogModel updateBlogModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _blogService.UpdateAsync(updateBlogModel);
            if (!result)
            {
                return NotFound($"Blog với ID {updateBlogModel.BlogId} không tồn tại.");
            }

            return Ok($"Cập nhật Blog với ID {updateBlogModel.BlogId} thành công.");
        }

        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Xóa Blog: Admin")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var result = await _blogService.RemoveAsync(id);
            if (!result)
            {
                return NotFound($"Blog với ID {id} không tồn tại.");
            }

            return Ok($"Xóa Blog với ID {id} thành công.");
        }

        [HttpGet("GetBlogListForCustomer")]
        [SwaggerOperation(Summary = "Danh sách Blog: Customer")]
        public async Task<IActionResult> CustomerBlogList(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? blogGroupId = null,
            [FromQuery] string? title = null)
        {
            var result = await _blogService.CustomerBlogListAsync(pageNumber, pageSize, blogGroupId, title);
            var response = new PaginatedResponseDto<BlogListCustomerDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );
            return Ok(response);
        }


        [HttpGet("GetBlogListForAdmin")]
        [SwaggerOperation(Summary = "Danh sách Blog: Admin")]
        public async Task<IActionResult> AdminBlogList(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? blogGroupId = null,
            [FromQuery] string? title = null)
        {
            var result = await _blogService.AdminBlogListAsync(pageNumber, pageSize, blogGroupId, title);
            var response = new PaginatedResponseDto<BlogAdminDto>(
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
