using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.DashboardRestaurant;

namespace TOPDER.Service.Dtos.DashboardAdmin
{
    public class DashboardAdminDTO
    {
        public OrderStatusDTO OrderStatus { get; set; } = null!;
        public TaskBarDTO TaskBar { get; set; } = null!;
        public List<TopRestaurantDTO> TopRestaurantDTOs { get; set; } = null!;
        public CustomerAgeGroupAdminDTO CustomerAgeGroup { get; set; } = null!;
        public List<ChartCategoryRestaurantDTO> ChartCategoryRestaurants { get; set; } = null!;
        public MarketOverviewOrderAdminDTO MarketOverviewOrderAdmin { get; set; } = null!;
        public MarketOverviewTotalInComeAdminDTO marketOverviewTotalInComeAdmin { get; set; } = null!;

    }
}
