using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.Discount;
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

        [HttpGet("customer/detail/{id}")]
        public async Task<IActionResult> GetBlog(int id)
        {
            try
            {
                var blog = await _blogService.GetBlogByIdAsync(id);
                return Ok(blog);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Blog với ID {id} không tồn tại.");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBlog([FromBody] CreateBlogModel createBlogModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _blogService.AddAsync(createBlogModel);
            if (result)
            {
                return Ok($"Tạo Blog thành công.");
            }

            return BadRequest("Tạo Blog thất bại.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateBlog([FromBody] UpdateBlogModel updateBlogModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _blogService.UpdateAsync(updateBlogModel);

            if (result)
            {
                return Ok($"Cập nhật Blog thành công.");
            }

            return NotFound($"Blog với ID {updateBlogModel.BlogId} không tồn tại.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var result = await _blogService.RemoveAsync(id);
            if (result)
            {
                return Ok($"Xóa Blog thành công.");
            }

            return NotFound($"Blog với ID {id} không tồn tại.");
        }

        [HttpGet("admin/list")]
        public async Task<IActionResult> GetBlogs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _blogService.GetPagingAsync(pageNumber, pageSize);

            var response = new PaginatedResponseDto<BlogAdminDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpGet("customer/detail/new-blog")]
        public async Task<IActionResult> GetNewBlogs()
        {
            var newBlogs = await _blogService.GetNewBlogAsync();
            return Ok(newBlogs);
        }

        [HttpGet("customer/search")]
        public async Task<IActionResult> GetBlogsByGroup([FromQuery] int blogGroupId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _blogService.SearchBlogByGroupPagingAsync(pageNumber, pageSize, blogGroupId);

            var response = new PaginatedResponseDto<BlogListCustomerDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpGet("admin/search")]
        public async Task<IActionResult> SearchBlogs([FromQuery] int blogGroupId, [FromQuery] string blogGroupName, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _blogService.SearchPagingAsync(pageNumber, pageSize, blogGroupId, blogGroupName);

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
