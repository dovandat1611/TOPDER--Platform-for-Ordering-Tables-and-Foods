using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Restaurant
{
    public class DescriptionRestaurant
    {
        public int RestaurantId { get; set; }
        public string? Description { get; set; }
        public string? Subdescription { get; set; }
    }
}
