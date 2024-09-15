using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
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
        public decimal? TotalPrice { get; set; }
        public string? ContentPayment { get; set; }
        public string? StatusPayment { get; set; }
        public string? StatusOrder { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? CancelledDate { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Discount? Discount { get; set; }
        public virtual Restaurant? Restaurant { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
