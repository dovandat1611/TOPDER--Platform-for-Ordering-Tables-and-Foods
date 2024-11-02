using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class RestaurantTable
    {
        public RestaurantTable()
        {
            OrderTables = new HashSet<OrderTable>();
            TableBookingSchedules = new HashSet<TableBookingSchedule>();
        }

        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public int? RoomId { get; set; }
        public string TableName { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public string? Description { get; set; }
        public bool? IsBookingEnabled { get; set; }
        public bool? IsVisible { get; set; }


        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual RestaurantRoom? Room { get; set; }
        public virtual ICollection<OrderTable> OrderTables { get; set; }
        public virtual ICollection<TableBookingSchedule> TableBookingSchedules { get; set; }
    }
}
