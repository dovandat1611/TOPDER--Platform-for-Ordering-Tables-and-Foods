using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.Dashboard;
using TOPDER.Service.Dtos.Discount;

namespace TOPDER.Service.IServices
{
    public interface IDashboardService
    {
        Task<DashboardAdminDTO> GetDashboardAdminAsync();
        Task<DashboardRestaurantDto> GetDashboardRestaurantAsync(int restaurantId);
        Task<MarketOverviewDTO> GetMarketOverviewAdminAsync(IQueryable<Order> orders, int? filteredYear);
        Task<MarketOverviewDTO> GetMarketOverviewRestaurantAsync(int restaurantId, int? filteredYear);
        Task<TaskBarMonthRestaurantDTO> GetTaskBarMonthDataAsync(int restaurantId, DateTime? searchMonth);
        Task<TaskBarDayRestaurantDTO> GetTaskBarDayDataAsync(int restaurantId, DateTime? searchDay);
    } 
}
