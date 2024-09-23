using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class CategoryRoom
    {
        public CategoryRoom()
        {
            Orders = new HashSet<Order>();
            RestaurantRooms = new HashSet<RestaurantRoom>();
        }

        public int CategoryRoomId { get; set; }
        public string CategoryName { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RestaurantRoom> RestaurantRooms { get; set; }
    }
}
