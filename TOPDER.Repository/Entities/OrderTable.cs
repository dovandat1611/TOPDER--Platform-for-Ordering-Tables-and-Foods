using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class OrderTable
    {
        public int OrderTableId { get; set; }
        public int OrderId { get; set; }
        public int TableId { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual RestaurantTable Table { get; set; } = null!;
    }
}
