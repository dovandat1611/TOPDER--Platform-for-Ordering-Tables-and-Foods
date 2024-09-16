using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class CategoryRestaurant
    {
        public CategoryRestaurant()
        {
            Restaurants = new HashSet<Restaurant>();
        }

        public int CategoryRestaurantId { get; set; }
        public string? CategoryRestaurantName { get; set; }

        public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}
