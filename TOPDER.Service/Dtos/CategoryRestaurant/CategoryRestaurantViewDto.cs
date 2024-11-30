using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.CategoryRestaurant
{
    public class CategoryRestaurantViewDto
    {
        public int CategoryRestaurantId { get; set; }
        public string? CategoryRestaurantName { get; set; }
        public bool IsDelete { get; set; }
    }
}
