using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Dtos.RestaurantRoom;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantRoomController : ControllerBase
    {
        private readonly IRestaurantRoomService _restaurantRoomService;

        public RestaurantRoomController(IRestaurantRoomService restaurantRoomService)
        {
            _restaurantRoomService = restaurantRoomService;
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo Phòng: Restaurant")]
        public async Task<IActionResult> Add([FromBody] CreateRestaurantRoomDto restaurantRoomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _restaurantRoomService.AddAsync(restaurantRoomDto);
            if (!result)
            {
                return BadRequest("Không thể thêm phòng.");
            }

            return Ok("Tạo phòng thành công");
        }

        [HttpGet("GetRoom/{restaurantId}/{roomId}")]
        [SwaggerOperation(Summary = "Lấy thông tin phòng của nhà hàng để cập nhật: Restaurant")]
        public async Task<IActionResult> GetItem(int restaurantId, int roomId)
        {
            try
            {
                var room = await _restaurantRoomService.GetItemAsync(roomId, restaurantId);
                return Ok(room);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập nhật phòng: Restaurant")]
        public async Task<IActionResult> Update([FromBody] UpdateRestaurantRoomDto restaurantRoomDto)
        {
            var result = await _restaurantRoomService.UpdateAsync(restaurantRoomDto);
            if (!result)
            {
                return NotFound("Không tìm thấy phòng để cập nhật.");
            }
            return Ok("Update Room thành công");
        }

        [HttpPut("Invisible/{restaurantId}/{roomId}")]
        [SwaggerOperation(Summary = "Ẩn/Xóa phòng: Restaurant")]
        public async Task<IActionResult> SetInvisible(int restaurantId, int roomId)
        {
            var result = await _restaurantRoomService.InvisibleAsync(roomId, restaurantId);
            if (!result)
            {
                return NotFound("Không tìm thấy phòng để xóa.");
            }
            return Ok($"Ẩn/Xóa Room và các bảng liên quan thành công.");
        }

        [HttpGet("GetRoomList/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy danh sách thông tin phòng của nhà hàng: Restaurant")]
        public async Task<IActionResult> SearchPaging(int restaurantId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] int? roomId = null, [FromQuery] string? roomName = null)
        {
            var result = await _restaurantRoomService.GetRoomListAsync(pageNumber, pageSize, restaurantId, roomId, roomName);
            var response = new PaginatedResponseDto<RestaurantRoomDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );
            return Ok(response);
        }

        [HttpPut("IsEnabledBooking/{restaurantId}/{roomId}")]
        [SwaggerOperation(Summary = "Thay đổi trạng thái Booking của Room: Restaurant")]
        public async Task<IActionResult> IsEnabledBooking(int restaurantId, int roomId, bool isEnabledBooking)
        {
            try
            {
                var result = await _restaurantRoomService.IsEnabledBookingAsync(roomId, restaurantId, isEnabledBooking);
                if (!result)
                {
                    return BadRequest("Không có sự thay đổi trạng thái đặt phòng.");
                }

                return Ok($"Thay đổi trạng thái IsEnabledBooking: {isEnabledBooking} thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
