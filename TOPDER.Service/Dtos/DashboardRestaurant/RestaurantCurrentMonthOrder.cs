using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.DashboardRestaurant
{
    public class RestaurantCurrentMonthOrder
    {
        public int CurrentMonthOrder { get; set; }
        public double OrderGrowthRate { get; set; }
    }
}
