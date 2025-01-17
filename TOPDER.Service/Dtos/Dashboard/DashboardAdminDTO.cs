﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Dashboard
{
    public class DashboardAdminDTO
    {
        public OrderStatusDTO OrderStatus { get; set; } = null!;
        public TaskBarAdminDTO TaskBar { get; set; } = null!;
        public List<TopRestaurantDTO> TopRestaurantDTOs { get; set; } = null!;
        public CustomerAgeGroupDTO CustomerAgeGroup { get; set; } = null!;
        public List<ChartCategoryRestaurantDTO> ChartCategoryRestaurants { get; set; } = null!;
        public MarketOverviewDTO MarketOverview { get; set; } = null!;
        public List<int> YearsContainOrders { get; set; } = new List<int> { };
    }
}
