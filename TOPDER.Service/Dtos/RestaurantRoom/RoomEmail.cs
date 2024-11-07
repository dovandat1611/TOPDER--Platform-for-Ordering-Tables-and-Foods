using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.RestaurantRoom
{
    public class RoomEmail
    {
        public string RoomName { get; set; } = string.Empty;
        public List<string> Tables { get; set; } = new List<string>();
    }
}
