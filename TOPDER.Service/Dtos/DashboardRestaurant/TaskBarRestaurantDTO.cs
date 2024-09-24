using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.DashboardAdmin;

namespace TOPDER.Service.Dtos.DashboardRestaurant
{
    public class TaskBarRestaurantDTO
    {
        public int TotalOrder { get; set; } 
        public double TotalIncome { get; set; } 
        public int Star { get; set; } 
        public bool RestaurantBookingStatus { get; set; }
        public RestaurantCurrentMonthIncome CurrentMonthIncome { get; set; } = null!;
        public RestaurantCurrentMonthOrder CurrentMonthOrder { get; set; } = null!;
    }
}
