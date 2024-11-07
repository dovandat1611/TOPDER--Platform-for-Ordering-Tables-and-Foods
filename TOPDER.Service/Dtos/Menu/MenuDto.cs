using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Menu
{
    public class MenuDto
    {
        public int MenuId { get; set; }
        public int RestaurantId { get; set; }
        public int? CategoryMenuId { get; set; }
        public string DishName { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
    }
}
