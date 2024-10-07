using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Dashboard
{
    public class MarketOverviewDTO
    {
        // Tổng doanh thu và tốc độ tăng trưởng doanh thu trong năm
        public double TotalInComeForYear { get; set; }
        public double TotalInComeGrowthRateForYear { get; set; }

        // Tổng đơn hàng và tốc độ tăng trưởng đơn hàng trong năm
        public int OrderForYear { get; set; }
        public double OrderGrowthRateForYear { get; set; }

        // Dữ liệu biểu đồ theo tháng, kết hợp cả tổng đơn hàng và doanh thu
        public List<ChartDTO> MonthlyData { get; set; } = new List<ChartDTO>();
    }
}
