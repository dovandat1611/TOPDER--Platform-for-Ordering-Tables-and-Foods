using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.RestaurantRoom
{
    public class RestaurantRoomDto
    {
        public int RoomId { get; set; }
        public int RestaurantId { get; set; }
        public int CategoryRoomId { get; set; }
        public string RoomName { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public string? Description { get; set; }
        public bool? IsBookingEnabled { get; set; }
    }
}
