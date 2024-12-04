using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
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

        [HttpGet("{id}")]
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

        [HttpPut]
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var result = await _blogService.RemoveAsync(id);
            if (!result)
            {
                return NotFound($"Blog với ID {id} không tồn tại.");
            }

            return Ok($"Xóa Blog với ID {id} thành công.");
        }

     /*   [HttpGet("customer/list")]
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
*/

        [HttpGet("admin/list")]
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
