using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Dashboard
{
    public class TaskBarAdminDTO
    {
        // Tổng từ database
        public int TotalOrder { get; set; }
        public int TotalCustomer { get; set; }
        public int TotalRestaurant { get; set; }
        public double TotalIncome { get; set; }

        public CurrentMonthIncomeDTO CurrentMonthIncome { get; set; } = null!;
        public CurrentMonthOrderDTO CurrentMonthOrder { get; set; } = null!;


    }

}
