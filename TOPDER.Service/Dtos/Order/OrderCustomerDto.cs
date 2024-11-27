using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.OrderMenu;
using TOPDER.Service.Dtos.OrderTable;

namespace TOPDER.Service.Dtos.Order
{
    public class OrderCustomerDto
    {
        public int OrderId { get; set; }
        public int? RestaurantId { get; set; }
        public string RestaurantName { get; set; } = null!;
        public string RestaurantPhone { get; set; } = null!;
        public string NameReceiver { get; set; } = null!;
        public string PhoneReceiver { get; set; } = null!;
        public TimeSpan TimeReservation { get; set; }
        public DateTime DateReservation { get; set; }
        public int NumberPerson { get; set; }
        public int NumberChild { get; set; }
        public string? ContentReservation { get; set; }
        public string? TypeOrder { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ContentPayment { get; set; }
        public string? StatusPayment { get; set; }
        public string? StatusOrder { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public bool IsFeedback { get; set; }
        public string? CancelReason { get; set; }
        public string? PaidType { get; set; }
        public decimal? DepositAmount { get; set; }
        public decimal? FoodAmount { get; set; }
        public decimal? FoodAddAmount { get; set; }
    }
}
