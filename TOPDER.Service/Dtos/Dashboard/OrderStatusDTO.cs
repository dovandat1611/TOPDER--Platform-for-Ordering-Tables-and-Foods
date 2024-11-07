using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Dashboard
{
    public class OrderStatusDTO
    {
        public int TotalOrder { get; set; }
        public int Pending { get; set; }
        public int Confirm { get; set; }
        public int Paid { get; set; }
        public int Complete { get; set; }
        public int Cancel { get; set; }
    }
}
