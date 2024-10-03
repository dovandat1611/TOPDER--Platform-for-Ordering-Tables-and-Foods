using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantTablesController : ControllerBase
    {
        private readonly IRestaurantTableService _restaurantTableService;  
        public RestaurantTablesController(IRestaurantTableService restaurantTableService)
        {
            _restaurantTableService = restaurantTableService;
        }

        // API để lấy danh sách bàn có sẵn
        [HttpGet("available-tables")]
        public async Task<IActionResult> GetAvailableTablesAsync(int restaurantId, string timeReservation, DateTime dateReservation)
        {
            // Chuyển đổi timeReservation từ string thành TimeSpan
            if (!TimeSpan.TryParse(timeReservation, out var parsedTimeReservation))
            {
                return BadRequest("Thời gian đặt bàn không hợp lệ.");
            }

            try
            {
                // Gọi service để lấy danh sách bàn có sẵn
                var availableTables = await _restaurantTableService.GetAvailableTablesAsync(restaurantId, parsedTimeReservation, dateReservation);

                return Ok(availableTables);  // Trả về danh sách bàn có sẵn
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về lỗi 500
                return StatusCode(500, "Đã xảy ra lỗi khi lấy danh sách bàn: " + ex.Message);
            }
        }

    }
}
