using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.DashboardRestaurant;

namespace TOPDER.Service.Dtos.DashboardAdmin
{
    public class MarketOverviewTotalInComeAdminDTO
    {
        public double TotalInComeForYear { get; set; }
        public double TotalInComeGrowthRateForYear { get; set; }
        public List<ChartTotalInComeDTO> MonthlyTotalInComeData { get; set; } = null!;
    }
}
