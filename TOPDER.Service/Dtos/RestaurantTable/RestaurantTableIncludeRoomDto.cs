using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.RestaurantRoom;


namespace TOPDER.Service.Dtos.RestaurantTable
{
    public class RestaurantTableIncludeRoomDto
    {
        public RestaurantRoomDto RestaurantRoom { get; set; } = new RestaurantRoomDto();
        public List<RestaurantTableCustomerDto> restaurantTables { get; set; } = new List<RestaurantTableCustomerDto>();
    }
}
