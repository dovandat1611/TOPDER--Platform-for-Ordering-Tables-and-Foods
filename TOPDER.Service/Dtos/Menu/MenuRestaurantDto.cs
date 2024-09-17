using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Menu
{
    public class MenuRestaurantDto
    {
        public int MenuId { get; set; }
        public int? CategoryMenuName { get; set; }
        public string DishName { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
    }
}
