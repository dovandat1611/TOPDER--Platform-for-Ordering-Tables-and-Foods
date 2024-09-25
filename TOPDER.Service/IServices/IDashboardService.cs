using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Dashboard;
using TOPDER.Service.Dtos.Discount;

namespace TOPDER.Service.IServices
{
    public interface IDashboardService
    {
        Task<DashboardAdminDTO> GetDashboardAdminAsync();
        Task<DashboardRestaurantDto> GetDashboardRestaurantAsync(int restaurantId);
    } 
}
