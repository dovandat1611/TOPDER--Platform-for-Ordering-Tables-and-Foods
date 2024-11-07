using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Wishlist
{
    public class UserWishlistDto
    {
        public int WishlistId { get; set; }
        public int? Uid { get; set; }
        public string Logo { get; set; } = string.Empty;
        public string NameRes { get; set; } = string.Empty;
        public int CategoryRestaurantId { get; set; } 
        public string CategoryName { get; set; } = string.Empty;
        public decimal? Discount { get; set; }
        public decimal Price { get; set; }
        public int TotalFeedbacks { get; set; }
        public int Star { get; set; }
    }
}
