using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class CategoryMenu
    {
        public CategoryMenu()
        {
            Menus = new HashSet<Menu>();
        }

        public int CategoryMenuId { get; set; }
        public string? CategoryMenuName { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
    }
}
