using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.DashboardRestaurant
{
    public class MarketOverviewOrderRestaurantDTO
    {
        public int OrderForYear { get; set; }
        public double OrderGrowthRateForYear { get; set; }
        public List<ChartOrderRestaurantDTO> MonthlyOrderData { get; set; } = null!;
    }
}
