using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Dtos.TableBookingSchedule;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableBookingScheduleController : ControllerBase
    {
        private readonly ITableBookingScheduleService _tableBookingScheduleService;

        public TableBookingScheduleController(ITableBookingScheduleService tableBookingScheduleService)
        {
            _tableBookingScheduleService = tableBookingScheduleService;
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Thêm mới lịch đặt bàn")]
        public async Task<IActionResult> AddTableBookingSchedule([FromBody] CreateTableBookingScheduleDto restaurantTableDto)
        {   
            if(!restaurantTableDto.TableIds.Any())
            {
                return BadRequest("TableId không được null.");
            }

            bool isAdded = await _tableBookingScheduleService.AddAsync(restaurantTableDto);
            if (isAdded)
            {
                return Ok("Thêm lịch đặt bàn thành công.");
            }
            return BadRequest("Thêm lịch đặt bàn thất bại.");
        }

        [HttpGet("GetTableBookingSchedule/{id}")]
        [SwaggerOperation(Summary = "Lấy thông tin lịch đặt bàn theo ID : Để Update")]
        public async Task<IActionResult> GetTableBookingSchedule(int id)
        {
            try
            {
                var tableBookingSchedule = await _tableBookingScheduleService.GetItemAsync(id);
                return Ok(tableBookingSchedule);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("GetTableBookingScheduleList/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy danh sách lịch đặt bàn theo nhà hàng")]
        public async Task<IActionResult> GetTableBookingSchedules(int restaurantId)
        {
            var tableBookingSchedules = await _tableBookingScheduleService.GetTableBookingScheduleListAsync(restaurantId);
            return Ok(tableBookingSchedules);
        }

        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Xóa lịch đặt bàn theo ID")]
        public async Task<IActionResult> RemoveTableBookingSchedule(int id)
        {
            bool isRemoved = await _tableBookingScheduleService.RemoveAsync(id);
            if (isRemoved)
            {
                return Ok("Xóa lịch đặt bàn thành công.");
            }
            return NotFound("Không tìm thấy lịch đặt bàn.");
        }

        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập nhật lịch đặt bàn")]
        public async Task<IActionResult> UpdateTableBookingSchedule([FromBody] TableBookingScheduleDto tableBookingSchedule)
        {
            bool isUpdated = await _tableBookingScheduleService.UpdateAsync(tableBookingSchedule);
            if (isUpdated)
            {
                return Ok("Cập nhật lịch đặt bàn thành công.");
            }
            return NotFound("Không tìm thấy lịch đặt bàn để cập nhật.");
        }
    }
}
