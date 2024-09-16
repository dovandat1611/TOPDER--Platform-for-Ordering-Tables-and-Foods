using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Menu
    {
        public Menu()
        {
            OrderMenus = new HashSet<OrderMenu>();
        }

        public int MenuId { get; set; }
        public int RestaurantId { get; set; }
        public int? CategoryMenuId { get; set; }
        public string DishName { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Status { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }

        public virtual CategoryMenu? CategoryMenu { get; set; }
        public virtual Restaurant Restaurant { get; set; } = null!;
        public virtual ICollection<OrderMenu> OrderMenus { get; set; }
    }
}
