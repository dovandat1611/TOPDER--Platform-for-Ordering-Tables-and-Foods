using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.DiscountMenu;

namespace TOPDER.Service.Dtos.Discount
{
    public class DiscountDto
    {
        public int DiscountId { get; set; }
        public int RestaurantId { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string DiscountName { get; set; } = null!;
        public string ApplicableTo { get; set; } = null!;
        public string ApplyType { get; set; } = null!;
        public decimal? MinOrderValue { get; set; }
        public decimal? MaxOrderValue { get; set; }
        public string Scope { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public int Quantity { get; set; }
        public List<CreateDiscountMenuDto> discountMenuDtos { get; set; }  = new List<CreateDiscountMenuDto>();
    }
}
