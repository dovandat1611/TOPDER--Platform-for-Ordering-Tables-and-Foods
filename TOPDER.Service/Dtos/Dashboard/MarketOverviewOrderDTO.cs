using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Dashboard
{
    public class MarketOverviewOrderDTO
    {
        public int OrderForYear { get; set; }
        public double OrderGrowthRateForYear { get; set; }
        public List<ChartOrderDTO> MonthlyOrderData { get; set; } = null!;

    }
}
