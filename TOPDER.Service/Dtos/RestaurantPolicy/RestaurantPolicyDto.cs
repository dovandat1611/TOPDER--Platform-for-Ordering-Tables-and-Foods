using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.RestaurantPolicy
{
    public class RestaurantPolicyDto
    {
        public int PolicyId { get; set; }
        public int RestaurantId { get; set; }
        public decimal? FirstFeePercent { get; set; }
        public decimal? ReturningFeePercent { get; set; }
        public decimal? CancellationFeePercent { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateDate { get; set; }
    }
}
