using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.DiscountMenu
{
    public class DiscountMenuDto
    {
        public int DiscountMenuId { get; set; }
        public int DiscountId { get; set; }
        public int MenuId { get; set; }
        public decimal DiscountMenuPercentage { get; set; }
    }
}
