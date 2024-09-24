using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.DashboardAdmin
{
    public class AdminCurrentMonthOrder
    {
        public int CurrentMonthOrder { get; set; }
        public double OrderGrowthRate { get; set; }
    }
}
