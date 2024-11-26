using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class RestaurantRoom
    {
        public RestaurantRoom()
        {
            RestaurantTables = new HashSet<RestaurantTable>();
        }

        public int RoomId { get; set; }
        public int RestaurantId { get; set; }
        public string RoomName { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public string? Description { get; set; }
        public bool? IsBookingEnabled { get; set; }
        public bool? IsVisible { get; set; }

        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual ICollection<RestaurantTable> RestaurantTables { get; set; }
    }
}
