using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.DiscountMenu
{
    public class CreateDiscountMenuDto
    {
        public int MenuId { get; set; }
        public decimal DiscountMenuPercentage { get; set; }
    }
}
