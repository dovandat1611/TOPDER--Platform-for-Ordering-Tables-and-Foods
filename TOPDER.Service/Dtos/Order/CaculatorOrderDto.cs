using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.OrderMenu;

namespace TOPDER.Service.Dtos.Order
{
    public class CaculatorOrderDto
    {
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }
        public List<OrderMenuModelDto>? OrderMenus { get; set; }
    }
}
