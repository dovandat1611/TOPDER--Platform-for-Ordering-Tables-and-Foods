using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.RestaurantPolicy
{
    public class CreateRestaurantPolicyDto
    {
        public int RestaurantId { get; set; }
        public decimal? FirstFeePercent { get; set; }
        public decimal? ReturningFeePercent { get; set; }
        public decimal? CancellationFeePercent { get; set; }
    }
}
