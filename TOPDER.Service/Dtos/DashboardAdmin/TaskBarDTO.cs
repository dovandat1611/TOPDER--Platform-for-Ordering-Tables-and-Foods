using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.DashboardAdmin
{
    public class TaskBarDTO
    {
        // Tổng từ database
        public int TotalOrder { get; set; }
        public int TotalCustomer { get; set; }
        public int TotalRestaurant { get; set; }
        public double TotalIncome { get; set; }

        public AdminCurrentMonthIncome CurrentMonthIncome { get; set; } = null!;
        public AdminCurrentMonthOrder CurrentMonthOrder { get; set; } = null!;


    }

}
