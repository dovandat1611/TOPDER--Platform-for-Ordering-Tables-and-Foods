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
        public string Image { get; set; } = string.Empty;
        public string NameRes { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int TotalReviews { get; set; }
        public int Star { get; set; }
    }
}
