using System;
using System.Collections.Generic;
using TOPDER.Repository.Entities;

namespace TOPDER.Repository.Entities
{
    public partial class Order
    {
        public Order()
        {
            Feedbacks = new HashSet<Feedback>();
            OrderMenus = new HashSet<OrderMenu>();
            OrderTables = new HashSet<OrderTable>();
        }

        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? RestaurantId { get; set; }
        public int? DiscountId { get; set; }
        public string NameReceiver { get; set; } = null!;
        public string PhoneReceiver { get; set; } = null!;
        public TimeSpan TimeReservation { get; set; }
        public DateTime DateReservation { get; set; }
        public int NumberPerson { get; set; }
        public int NumberChild { get; set; }
        public string? ContentReservation { get; set; }
        public string? TypeOrder { get; set; }
        public string? PaidType { get; set; }
        public decimal? DepositAmount { get; set; }
        public decimal? FoodAmount { get; set; }
        public decimal? FoodAddAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? ContentPayment { get; set; }
        public string? StatusPayment { get; set; }
        public string? StatusOrder { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancelReason { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Discount? Discount { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<OrderMenu> OrderMenus { get; set; }
        public virtual ICollection<OrderTable> OrderTables { get; set; }
    }
}
