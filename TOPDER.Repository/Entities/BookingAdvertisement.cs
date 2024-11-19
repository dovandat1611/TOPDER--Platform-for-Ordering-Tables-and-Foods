using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class BookingAdvertisement
    {
        public int BookingId { get; set; }
        public int RestaurantId { get; set; }
        public string? Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string? StatusPayment { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Restaurant Restaurant { get; set; } = null!;
    }
}
