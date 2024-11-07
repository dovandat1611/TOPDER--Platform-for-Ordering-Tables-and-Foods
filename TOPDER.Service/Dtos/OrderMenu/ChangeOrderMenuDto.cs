using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.OrderMenu
{
    public class ChangeOrderMenuDto
    {
        public int RestaurantId { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public List<OrderMenuModelDto> orderMenus { get; set; } = new List<OrderMenuModelDto>();
    }
}
