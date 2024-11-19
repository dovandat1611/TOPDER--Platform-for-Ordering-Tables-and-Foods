using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Dashboard
{
    public class TaskBarMonthRestaurantDTO
    {
        public CurrentMonthIncomeDTO CurrentMonthIncome { get; set; } = null!;
        public CurrentMonthOrderDTO CurrentMonthOrder { get; set; } = null!;
    }
}
