using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("CreateRoom")]
        public async Task<IActionResult> Add([FromBody] RestaurantRoomDto restaurantRoomDto)
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

            return CreatedAtAction(nameof(GetItem), new { id = restaurantRoomDto.RoomId }, restaurantRoomDto);
        }

        [HttpGet("GetRoom/{id}/{restaurantId}")]
        public async Task<IActionResult> GetItem(int id, int restaurantId)
        {
            try
            {
                var room = await _restaurantRoomService.GetItemAsync(id, restaurantId);
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

        [HttpPut("UpdateRoom/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RestaurantRoomDto restaurantRoomDto)
        {
            if (id != restaurantRoomDto.RoomId || !ModelState.IsValid)
            {
                return BadRequest("Thông tin phòng không hợp lệ.");
            }

            var result = await _restaurantRoomService.UpdateAsync(restaurantRoomDto);
            if (!result)
            {
                return NotFound("Không tìm thấy phòng để cập nhật.");
            }

            return NoContent();
        }

        [HttpDelete("DeleteRoom/{id}/{restaurantId}")]
        public async Task<IActionResult> Remove(int id, int restaurantId)
        {
            var result = await _restaurantRoomService.RemoveAsync(id, restaurantId);
            if (!result)
            {
                return NotFound("Không tìm thấy phòng để xóa.");
            }

            return NoContent();
        }

        [HttpGet("GetRoomList/{restaurantId}")]
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

        [HttpPut("IsEnabledBooking/{roomId}/{restaurantId}")]
        public async Task<IActionResult> IsEnabledBooking(int roomId, int restaurantId, [FromBody] bool isEnabledBooking)
        {
            try
            {
                var result = await _restaurantRoomService.IsEnabledBookingAsync(roomId, restaurantId, isEnabledBooking);
                if (!result)
                {
                    return BadRequest("Không có sự thay đổi trạng thái đặt phòng.");
                }

                return NoContent();
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
