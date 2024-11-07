using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.Image;

namespace TOPDER.Service.Dtos.Restaurant
{
    public class RestaurantHomeDto
    {
        public List<RestaurantDto> TopBookingRestaurants { get; set; } = null!;
        public List<RestaurantDto> TopRatingRestaurant { get; set; } = null!;
        public List<RestaurantDto> NewRestaurants { get; set; } = null!;
        public List<BlogListCustomerDto> Blogs { get; set; } = null!;
    }
}
