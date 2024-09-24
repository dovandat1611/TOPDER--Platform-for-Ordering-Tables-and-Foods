using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.DashboardRestaurant
{
    public class OrderStatusRestaurantDTO
    {
        public int Wait { get; set; }
        public int Accept { get; set; }
        public int Process { get; set; }
        public int Done { get; set; }
        public int Cancel { get; set; }
    }
}
