using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Dtos.Wishlist;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Thêm nhà hàng vào Wishlist: Customer")]
        public async Task<IActionResult> Add([FromBody] WishlistDto wishlistDto)
        {
            if (wishlistDto == null)
            {
                return BadRequest("Invalid data.");
            }

            var result = await _wishlistService.AddAsync(wishlistDto);
            if (result)
            {
                return Ok($"Thêm nhà hàng với ID {wishlistDto.RestaurantId} vào Wishlist thành công.");
            }

            return Conflict("Wishlist item already exists.");
        }

        [HttpGet("GetWishlistList/{customerId}")]
        [SwaggerOperation(Summary = "lấy danh sách Wishlist của khách hàng: Customer")]
        public async Task<IActionResult> GetPaging(int customerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var paginatedWishlists = await _wishlistService.GetPagingAsync(pageNumber, pageSize, customerId);
            return Ok(paginatedWishlists);
        }

        [HttpDelete("Delete/{customerId}/{wishlistId}")]
        [SwaggerOperation(Summary = "Xóa danh sách yêu thích: Customer")]
        public async Task<IActionResult> Remove(int customerId,int wishlistId)
        {
            var result = await _wishlistService.RemoveAsync(wishlistId, customerId);
            if (result)
            {
                return Ok("Xóa nhà hàng ra khỏi Wishlist thành công");
            }
            return NotFound("Wishlist item not found or does not belong to the customer.");
        }

    }
}
