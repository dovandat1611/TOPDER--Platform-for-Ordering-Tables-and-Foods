using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.ChatBox
{
    public class CreateChatBoxDto
    {
        public int ChatBoxId { get; set; }
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }
    }
}
