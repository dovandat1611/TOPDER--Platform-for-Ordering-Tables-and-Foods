using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.OrderMenu
{
    public class CreateOrUpdateOrderMenuDto
    {
        public int OrderMenuId { get; set; }
        public int OrderId { get; set; }
        public int MenuId { get; set; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
