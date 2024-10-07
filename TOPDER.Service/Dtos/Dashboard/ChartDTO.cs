using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Dashboard
{
    public class ChartDTO
    {
        public int Month { get; set; } // Tháng
        public int TotalOrders { get; set; } // Tổng số đơn hàng
        public decimal TotalInComes { get; set; } // Tổng doanh thu
    }
}
