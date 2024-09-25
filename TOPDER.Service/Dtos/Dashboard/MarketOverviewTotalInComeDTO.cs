using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Dashboard;

namespace TOPDER.Service.Dtos.Dashboard
{
    public class MarketOverviewTotalInComeDTO
    {
        public double TotalInComeForYear { get; set; }
        public double TotalInComeGrowthRateForYear { get; set; }
        public List<ChartTotalInComeDTO> MonthlyTotalInComeData { get; set; } = null!;
    }
}
