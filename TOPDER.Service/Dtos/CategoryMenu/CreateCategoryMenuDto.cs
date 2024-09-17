using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.CategoryMenu
{
    public class CreateCategoryMenuDto
    {
        public int RestaurantId { get; set; }
        public string? CategoryMenuName { get; set; }
    }
}
