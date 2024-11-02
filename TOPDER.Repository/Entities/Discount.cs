using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Discount
    {
        public Discount()
        {
            DiscountMenus = new HashSet<DiscountMenu>();
            Orders = new HashSet<Order>();
        }

        public int DiscountId { get; set; }
        public int RestaurantId { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string DiscountName { get; set; } = null!;
        public string ApplicableTo { get; set; } = null!;
        public string ApplyType { get; set; } = null!;
        public decimal? MinOrderValue { get; set; }
        public decimal? MaxOrderValue { get; set; }
        public string Scope { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public int Quantity { get; set; }
        public bool? IsVisible { get; set; }


        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual ICollection<DiscountMenu> DiscountMenus { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
