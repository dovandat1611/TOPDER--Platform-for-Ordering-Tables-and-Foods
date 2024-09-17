using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.RestaurantTable
{
    public class RestaurantTableDto
    {
        public int TableId { get; set; }
        public int RestaurantId { get; set; }
        public string TableName { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool? IsBookingEnabled { get; set; }
    }
}
