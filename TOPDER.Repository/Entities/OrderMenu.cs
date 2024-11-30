using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class OrderMenu
    {
        public int OrderMenuId { get; set; }
        public int OrderId { get; set; }
        public int MenuId { get; set; }
        public int? Quantity { get; set; }
        public decimal Price { get; set; }
        public string? OrderMenuType { get; set; }

        public virtual Menu Menu { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}
