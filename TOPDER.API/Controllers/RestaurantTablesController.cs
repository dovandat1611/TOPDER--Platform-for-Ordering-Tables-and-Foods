﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo bàn: Restaurant")]
        public async Task<IActionResult> AddAsync([FromBody] RestaurantTableDto restaurantTableDto)
        {
            if (restaurantTableDto == null)
            {
                return BadRequest("Dữ liệu bàn ăn là bắt buộc.");
            }

            var result = await _restaurantTableService.AddAsync(restaurantTableDto);
            if (result)
            {
                return Ok("Tạo bàn thành công!");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi thêm bàn.");
        }

        [HttpGet("GetTable/{tableId}/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy thông tin của bàn để cập nhật: Restaurant")]
        public async Task<IActionResult> GetItemAsync(int tableId, int restaurantId)
        {
            try
            {
                var table = await _restaurantTableService.GetItemAsync(tableId, restaurantId);
                return Ok(table);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Không tìm thấy bàn với ID {tableId}.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("Không có quyền truy cập.");
            }
        }

        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập nhật bàn: Restaurant")]
        public async Task<IActionResult> UpdateAsync([FromBody] RestaurantTableDto restaurantTableDto)
        {
            if (restaurantTableDto == null)
            {
                return BadRequest("Dữ liệu bàn ăn là bắt buộc.");
            }

            var result = await _restaurantTableService.UpdateAsync(restaurantTableDto);
            if (result)
            {
                return Ok("Update table thành công");
            }

            return NotFound($"Không tìm thấy bàn với ID {restaurantTableDto.TableId} hoặc không thuộc nhà hàng đã chỉ định.");
        }


        [HttpPut("Invisible/{restaurantId}/{tableId}")]
        [SwaggerOperation(Summary = "Xóa/Ẩn bàn: Restaurant")]
        public async Task<IActionResult> SetInvisible(int restaurantId, int tableId)
        {
            var result = await _restaurantTableService.InvisibleAsync(tableId, restaurantId);
            if (result)
            {
                return Ok($"Ẩn/Xóa Table thành công.");
            }

            return NotFound($"Không tìm thấy bàn với ID {tableId} hoặc không thuộc nhà hàng đã chỉ định.");
        }

        [HttpGet("GetTableList/{restaurantId}")]
        [SwaggerOperation(Summary = "lấy ra danh sách tất cả các bàn của nhà hàng: Restaurant")]
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

        [HttpGet("GetTableScheduleList/{restaurantId}")]
        [SwaggerOperation(Summary = "lấy ra danh sách tất cả các bàn của nhà hàng: Restaurant")]
        public async Task<IActionResult> GetTableScheduleList(int restaurantId)
        {
            var result = await _restaurantTableService.GetTableScheduleAsync(restaurantId);
            return Ok(result);
        }


        // API để lấy danh sách bàn có sẵn
        [HttpGet("GetAvailableTables")]
        [SwaggerOperation(Summary = "Lấy ra những bàn hợp lệ ví dụ như thời gian, enable booking table (or room if exist): Customer | Restaurant")]
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

        [HttpPost("CreateByExcel")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Tạo một list bàn ăn thông qua file excel: Restaurant")]
        public async Task<IActionResult> AddRangeFromExcel([FromForm] CreateExcelRestaurantTableDto createExcelRestaurantTableDto)
        {
            var (isSuccess, message) = await _restaurantTableService.AddRangeExcelAsync(createExcelRestaurantTableDto);
            if (!isSuccess)
            {
                return StatusCode(500, message);
            }
            return Ok(message);
        }

        [HttpPut("IsEnabledBooking/{restaurantId}/{roomId}")]
        [SwaggerOperation(Summary = "Thay đổi trạng thái Booking của Table: Restaurant")]
        public async Task<IActionResult> IsEnabledBooking(int restaurantId,int tableId,[FromBody] bool isEnabledBooking)
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
