using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.DashboardRestaurant
{
    public class MarketOverviewTotalInComeRestaurantDTO
    {
        public double TotalInComeForYear { get; set; }
        public double TotalInComeGrowthRateForYear { get; set; }
        public List<ChartTotalInComeRestaurantDTO> MonthlyTotalInComeData { get; set; } = null!;
    }
}
