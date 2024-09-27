using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.BlogGroup;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogGroupController : ControllerBase
    {
        private readonly IBlogGroupService _blogGroupService;

        public BlogGroupController(IBlogGroupService blogGroupService)
        {
            _blogGroupService = blogGroupService;
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetBlogGroup(int id)
        {
            try
            {
                var blogGroup = await _blogGroupService.GetItemAsync(id);
                return Ok(blogGroup);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBlogGroup([FromBody] BlogGroupDto blogGroupDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _blogGroupService.AddAsync(blogGroupDto);
            if (result)
            {
                return Ok($"Tạo Blog Group thành công.");
            }

            return BadRequest("Tạo Blog Group thất bại.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateBlogGroup([FromBody] BlogGroupDto blogGroupDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _blogGroupService.UpdateAsync(blogGroupDto);
            if (result)
            {
                return Ok($"Cập nhật Blog Group với ID {blogGroupDto.BloggroupId} thành công.");
            }
            return NotFound($"Blog Group với ID {blogGroupDto.BloggroupId} không tồn tại.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogGroup(int id)
        {
            var result = await _blogGroupService.RemoveAsync(id);
            if (result)
            {
                return Ok($"Xóa Blog group với ID {id} thành công.");
            }
            return NotFound($"Blog group với ID {id} không tồn tại.");
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetBlogGroups([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _blogGroupService.GetPagingAsync(pageNumber, pageSize);

            var response = new PaginatedResponseDto<BlogGroupDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBlogGroups([FromQuery] string blogGroupName, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _blogGroupService.SearchPagingAsync(pageNumber, pageSize, blogGroupName);

            var response = new PaginatedResponseDto<BlogGroupDto>(
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
