﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Discount
{
    public class DiscountMenuForAvailableDiscountDto
    {
        public int DiscountMenuId { get; set; }
        public int DiscountId { get; set; }
        public int MenuId { get; set; }
        public string DishName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public decimal DiscountMenuPercentage { get; set; }
    }
}