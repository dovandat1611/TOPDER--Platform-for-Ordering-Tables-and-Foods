using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Email
{
    public class OrderConfirmationEmail
    {
        public string Name { get; set; }  = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string RestaurantName { get; set; } = string.Empty;
        public int NumberOfGuests { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public decimal TotalAmount { get; set; }
        public List<string> TableName { get; set; } = new List<string>();
    }
}
