using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.DashboardRestaurant;

namespace TOPDER.Service.Dtos.DashboardAdmin
{
    public class MarketOverviewOrderAdminDTO
    {
        public int OrderForYear { get; set; }
        public double OrderGrowthRateForYear { get; set; }
        public List<ChartOrderDTO> MonthlyOrderData { get; set; } = null!;

    }
}
