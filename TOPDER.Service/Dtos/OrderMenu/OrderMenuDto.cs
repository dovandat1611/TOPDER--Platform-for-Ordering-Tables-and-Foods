using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.OrderMenu
{
    public class OrderMenuDto
    {
        public int OrderMenuId { get; set; }
        public int OrderId { get; set; }
        public int MenuId { get; set; }
        public string MenuName { get; set; } = null!;
        public string MenuImage { get; set; } = null!;
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
        public string? OrderMenuType { get; set; }
    }
}
