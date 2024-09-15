using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Discount
    {
        public Discount()
        {
            Orders = new HashSet<Order>();
        }

        public int DiscountId { get; set; }
        public int RestaurantId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public int Quantity { get; set; }

        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; }
    }
}
