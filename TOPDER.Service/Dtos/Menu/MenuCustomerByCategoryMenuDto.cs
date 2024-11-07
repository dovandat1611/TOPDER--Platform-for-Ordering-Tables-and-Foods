using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Menu
{
    public class MenuCustomerByCategoryMenuDto
    {
        public int? CategoryMenuId { get; set; }
        public string? CategoryMenuName { get; set; }
        public List<MenuCustomerDto> MenusOfCategoryMenu { get; set; } = new List<MenuCustomerDto>();
    }
}
