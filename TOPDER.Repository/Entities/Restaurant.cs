using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Restaurant
    {
        public Restaurant()
        {
            CategoryMenus = new HashSet<CategoryMenu>();
            CategoryRooms = new HashSet<CategoryRoom>();
            ChatBoxes = new HashSet<ChatBox>();
            Discounts = new HashSet<Discount>();
            Feedbacks = new HashSet<Feedback>();
            Images = new HashSet<Image>();
            Menus = new HashSet<Menu>();
            Orders = new HashSet<Order>();
            RestaurantRooms = new HashSet<RestaurantRoom>();
            RestaurantTables = new HashSet<RestaurantTable>();
            Wishlists = new HashSet<Wishlist>();
        }

        public int Uid { get; set; }
        public int? CategoryRestaurantId { get; set; }
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
        public string? Discount { get; set; }
        public int MaxCapacity { get; set; }
        public decimal Price { get; set; }
        public bool? IsBookingEnabled { get; set; }
        public decimal? FirstFeePercent { get; set; }
        public decimal? ReturningFeePercent { get; set; }
        public decimal? CancellationFeePercent { get; set; }

        public virtual CategoryRestaurant? CategoryRestaurant { get; set; }
        public virtual User UidNavigation { get; set; } = null!;
        public virtual ICollection<CategoryMenu> CategoryMenus { get; set; }
        public virtual ICollection<CategoryRoom> CategoryRooms { get; set; }
        public virtual ICollection<ChatBox> ChatBoxes { get; set; }
        public virtual ICollection<Discount> Discounts { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<RestaurantRoom> RestaurantRooms { get; set; }
        public virtual ICollection<RestaurantTable> RestaurantTables { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
    }
}
