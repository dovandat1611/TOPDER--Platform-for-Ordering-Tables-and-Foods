using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Restaurant
{
    public class RestaurantHomeDto
    {
        public int ResId { get; set; }
        public string Image { get; set; } = null!;
        public string NameRes { get; set; } = null!;
        public int? CategoryRestaurantId { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal? Discount { get; set; }
        public decimal Price { get; set; }
        public int TotalFeedbacks { get; set; }
        public int Star { get; set; }
    }
}
