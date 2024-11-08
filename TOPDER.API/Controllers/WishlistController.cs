using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        // POST: api/wishlist
        [HttpPost]
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

        // GET: api/wishlist/{customerId}
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetPaging(int customerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var paginatedWishlists = await _wishlistService.GetPagingAsync(pageNumber, pageSize, customerId);
            return Ok(paginatedWishlists);
        }

    
    }
}
