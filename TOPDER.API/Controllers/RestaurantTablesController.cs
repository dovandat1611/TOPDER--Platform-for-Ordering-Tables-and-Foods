using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.RestaurantRoom;
using TOPDER.Service.Dtos.RestaurantTable;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;

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

        [HttpPost("CreateTable")]
        public async Task<IActionResult> AddAsync([FromBody] RestaurantTableDto restaurantTableDto)
        {
            if (restaurantTableDto == null)
            {
                return BadRequest("Restaurant table data is required.");
            }

            var result = await _restaurantTableService.AddAsync(restaurantTableDto);
            if (result)
            {
                return Ok("Tạo bàn thành công!");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the table.");
        }

        [HttpGet("GetTable/{tableId}/{restaurantId}")]
        public async Task<IActionResult> GetItemAsync(int tableId, int restaurantId)
        {
            try
            {
                var table = await _restaurantTableService.GetItemAsync(tableId, restaurantId);
                return Ok(table);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Table with ID {tableId} not found.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("UpdateTable")]
        public async Task<IActionResult> UpdateAsync([FromBody] RestaurantTableDto restaurantTableDto)
        {
            if (restaurantTableDto == null)
            {
                return BadRequest("Restaurant table data is required.");
            }

            var result = await _restaurantTableService.UpdateAsync(restaurantTableDto);
            if (result)
            {
                return NoContent();
            }

            return NotFound($"Table with ID {restaurantTableDto.TableId} not found or does not belong to the specified restaurant.");
        }

        [HttpDelete("DeleteTable/{tableId}/{restaurantId}")]
        public async Task<IActionResult> RemoveAsync(int tableId, int restaurantId)
        {
            var result = await _restaurantTableService.RemoveAsync(tableId, restaurantId);
            if (result)
            {
                return NoContent();
            }

            return NotFound($"Table with ID {tableId} not found or does not belong to the specified restaurant.");
        }

        [HttpGet("GetTableList/{restaurantId}")]
        public async Task<IActionResult> SearchPagingAsync(int restaurantId, int pageNumber, int pageSize, string? tableName)
        {
            var result = await _restaurantTableService.GetTableListAsync(pageNumber, pageSize, restaurantId, tableName);

            var response = new PaginatedResponseDto<RestaurantTableRestaurantDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        // API để lấy danh sách bàn có sẵn
        [HttpGet("GetAvailableTables")]
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

        [HttpPost("CreateByExcelTable")]
        public async Task<IActionResult> AddRangeExcel([FromForm] CreateExcelRestaurantTableDto createExcelRestaurantTableDto)
        {
            if (createExcelRestaurantTableDto.File == null || createExcelRestaurantTableDto.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var result = await _restaurantTableService.AddRangeExcelAsync(createExcelRestaurantTableDto);
            if (result)
            {
                return Ok("Tables added successfully.");
            }
            return BadRequest("Failed to add tables from Excel.");
        }

        [HttpPut("IsEnabledBooking/{roomId}/{restaurantId}")]
        public async Task<IActionResult> IsEnabledBooking(int tableId, int restaurantId, [FromBody] bool isEnabledBooking)
        {
            try
            {
                var result = await _restaurantTableService.IsEnabledBookingAsync(tableId, restaurantId, isEnabledBooking);
                if (!result)
                {
                    return BadRequest("Không có sự thay đổi trạng thái bàn.");
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
