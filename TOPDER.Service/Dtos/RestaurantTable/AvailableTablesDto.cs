using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.RestaurantTable
{
    public class AvailableTablesDto
    {
        public List<RestaurantTableIncludeRoomDto> TablesWithRooms { get; set; } = new List<RestaurantTableIncludeRoomDto>();
        public List<RestaurantTableCustomerDto> StandaloneTables { get; set; } = new List<RestaurantTableCustomerDto>();
    }
}
