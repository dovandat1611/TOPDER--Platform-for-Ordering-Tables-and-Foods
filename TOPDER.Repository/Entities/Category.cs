using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Category
    {
        public Category()
        {
            Restaurants = new HashSet<Restaurant>();
        }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}
