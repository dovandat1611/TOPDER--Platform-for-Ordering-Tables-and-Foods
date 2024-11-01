using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class TableBookingSchedule
    {
        public int ScheduleId { get; set; }
        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }

        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual RestaurantTable Table { get; set; } = null!;
    }
}
