using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class DiscountMenu
    {
        public int DiscountMenuId { get; set; }
        public int DiscountId { get; set; }
        public int MenuId { get; set; }
        public decimal DiscountMenuPercentage { get; set; }

        public virtual Discount Discount { get; set; } = null!;
        public virtual Menu Menu { get; set; } = null!;
    }
}
