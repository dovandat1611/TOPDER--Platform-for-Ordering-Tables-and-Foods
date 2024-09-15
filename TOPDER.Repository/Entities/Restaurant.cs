using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Restaurant
    {
        public Restaurant()
        {
            ChatBoxes = new HashSet<ChatBox>();
            Discounts = new HashSet<Discount>();
            Images = new HashSet<Image>();
            Logs = new HashSet<Log>();
            Menus = new HashSet<Menu>();
            Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
            Wishlists = new HashSet<Wishlist>();
        }

        public int Uid { get; set; }
        public int? CategoryId { get; set; }
        public string NameOwner { get; set; } = null!;
        public string NameRes { get; set; } = null!;
        public string? Logo { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Description { get; set; }
        public string? Subdescription { get; set; }
        public string Location { get; set; } = null!;
        public decimal? Price { get; set; }
        public string? Discount { get; set; }
        public bool? IsBookingEnabled { get; set; }
        public decimal? FirstFeePercent { get; set; }
        public decimal? ReturningFeePercent { get; set; }
        public decimal? CancellationFeePercent { get; set; }

        public virtual Category? Category { get; set; }
        public virtual User UidNavigation { get; set; } = null!;
        public virtual ICollection<ChatBox> ChatBoxes { get; set; }
        public virtual ICollection<Discount> Discounts { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
    }
}
