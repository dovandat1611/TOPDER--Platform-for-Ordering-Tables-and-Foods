using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.DashboardAdmin;

namespace TOPDER.Service.Dtos.DashboardRestaurant
{
    public class DashboardRestaurantDto
    {
        public TaskBarRestaurantDTO TaskBar { get; set; } = null!;
        public OrderStatusRestaurantDTO OrderStatus { get; set; } = null!;
        public List<TopLoyalCustomerDTO> LoyalCustomers { get; set; } = null!;
        public MarketOverviewOrderRestaurantDTO MarketOverviewOrderRestaurant { get; set; } = null!;
        public MarketOverviewTotalInComeRestaurantDTO marketOverviewTotalInComeRestaurant { get; set; } = null!;
        public CustomerAgeGroupRestaurantDTO CustomerAgeGroup { get; set; } = null!;
        public FeedbackStarDTO FeedbackStars { get; set; } = null!;
    }
}
