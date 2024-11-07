using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Dashboard
{
    public class TaskBarRestaurantDTO
    {
        public int TotalOrder { get; set; }
        public double TotalIncome { get; set; }
        public int Star { get; set; }
        public bool RestaurantBookingStatus { get; set; }
        public CurrentMonthIncomeDTO CurrentMonthIncome { get; set; } = null!;
        public CurrentMonthOrderDTO CurrentMonthOrder { get; set; } = null!;
    }
}
