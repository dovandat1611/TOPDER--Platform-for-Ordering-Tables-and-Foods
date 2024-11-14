using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Repository.IRepositories;
using TOPDER.Service.Dtos.Dashboard;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly IOrderRepository _orderRepository;

        public DashboardController(IDashboardService dashboardService, IOrderRepository orderRepository)
        {
            _dashboardService = dashboardService;
            _orderRepository = orderRepository;
        }

        [HttpGet("GetDashboardAdmin")]
        [SwaggerOperation(Summary = "Dashboard : Admin")]
        public async Task<ActionResult<DashboardAdminDTO>> GetDashboardAdmin()
        {
            try
            {
                var result = await _dashboardService.GetDashboardAdminAsync();
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetDashboardRestaurant/{restaurantId}")]
        [SwaggerOperation(Summary = "Dashboard : Restaurant")]
        public async Task<ActionResult<DashboardRestaurantDto>> GetDashboardRestaurant(int restaurantId)
        {
            try
            {
                var result = await _dashboardService.GetDashboardRestaurantAsync(restaurantId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // API lấy dữ liệu thống kê tháng
        [HttpGet("TaskBarSearchByMonthRestaurant/{restaurantId}")]
        public async Task<IActionResult> GetTaskBarMonthData(int restaurantId, [FromQuery] DateTime? searchMonth)
        {
            try
            {
                var result = await _dashboardService.GetTaskBarMonthDataAsync(restaurantId, searchMonth);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy dữ liệu thống kê tháng: {ex.Message}");
            }
        }

        // API lấy dữ liệu thống kê ngày
        [HttpGet("TaskBarSearchByDayRestaurant/{restaurantId}")]
        public async Task<IActionResult> GetTaskBarDayData(int restaurantId, [FromQuery] DateTime? searchDay)
        {
            try
            {
                var result = await _dashboardService.GetTaskBarDayDataAsync(restaurantId, searchDay);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy dữ liệu thống kê ngày: {ex.Message}");
            }
        }


        [HttpGet("restaurant/overview/{restaurantId}")]
        public async Task<IActionResult> GetMarketOverview(int restaurantId, [FromQuery] int? filteredYear)
        {
            // Gọi phương thức GetMarketOverviewRestaurantAsync
            var result = await _dashboardService.GetMarketOverviewRestaurantAsync(restaurantId, filteredYear);

            // Trả về kết quả
            return Ok(result); // Trả về HTTP 200 với kết quả
        }

        [HttpGet("admin/overview")]
        public async Task<IActionResult> GetMarketOverview([FromQuery] int? filteredYear)
        {
            // Fetch all orders using the repository
            var orders = await _orderRepository.QueryableAsync();

            // Gọi phương thức GetMarketOverviewAdminAsync
            var result = await _dashboardService.GetMarketOverviewAdminAsync(orders.AsQueryable(), filteredYear);

            // Trả về kết quả
            return Ok(result); // Trả về HTTP 200 với kết quả
        }


    }
}
