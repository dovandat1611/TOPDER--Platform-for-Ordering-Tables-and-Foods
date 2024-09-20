using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Wishlist
{
    public class UserWishlistDto
    {
        public int ResId { get; set; }
        public string Image { get; set; } = string.Empty;
        public string NameRes { get; set; } = string.Empty;
        public int CategoryId { get; set; } 
        public string CategoryName { get; set; } = string.Empty;
        public int TotalFeedbacks { get; set; }
        public int Star { get; set; }
    }
}
