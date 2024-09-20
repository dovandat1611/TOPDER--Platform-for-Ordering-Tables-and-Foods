using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Discount
{
    public class DiscountDto
    {
        public int DiscountId { get; set; }
        public int RestaurantId { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public int Quantity { get; set; }
    }
}
