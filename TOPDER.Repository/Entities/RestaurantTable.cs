using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class RestaurantTable
    {
        public RestaurantTable()
        {
            OrderTables = new HashSet<OrderTable>();
        }

        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public string TableName { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool? IsBookingEnabled { get; set; }

        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual ICollection<OrderTable> OrderTables { get; set; }
    }
}
