using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Dashboard
{
    public class DashboardRestaurantDto
    {
        public TaskBarRestaurantDTO TaskBar { get; set; } = null!;
        public OrderStatusDTO OrderStatus { get; set; } = null!;
        public List<TopLoyalCustomerDTO> LoyalCustomers { get; set; } = null!;
        public CustomerAgeGroupDTO CustomerAgeGroup { get; set; } = null!;
        public FeedbackStarDTO FeedbackStars { get; set; } = null!;
        public MarketOverviewDTO MarketOverview { get; set; } = null!;
        public List<int> YearsContainOrders { get; set; } = new List<int> {};
    }
}
