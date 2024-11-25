using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.ChatBox
{
    public class ChatBoxDto
    {
        public int ChatBoxId { get; set; }
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerImage { get; set; } = null!;
        public string RestaurantName { get; set; } = null!;
        public string RestaurantImage { get; set; } = null!;
        public bool? IsRead { get; set; }
    }
}
