using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Repository.Entities;
using TOPDER.Service.Dtos.OrderMenu;

namespace TOPDER.Service.Dtos.Order
{
    public class OrderModel
    {
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }
        public int? DiscountId { get; set; }
        public int? CategoryRoomId { get; set; }
        public string NameReceiver { get; set; } = null!;
        public string PhoneReceiver { get; set; } = null!;
        public TimeSpan TimeReservation { get; set; }
        public DateTime DateReservation { get; set; }
        public int NumberPerson { get; set; }
        public int NumberChild { get; set; }
        public string? ContentReservation { get; set; }
        public List<OrderMenuModelDto>? OrderMenus { get; set; }
        public List<int> TableIds { get; set; } = new List<int>();
    }
}
