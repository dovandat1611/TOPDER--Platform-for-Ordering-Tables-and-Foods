using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class RestaurantPolicy
    {
        public int PolicyId { get; set; }
        public int RestaurantId { get; set; }
        public decimal? FirstFeePercent { get; set; }
        public decimal? ReturningFeePercent { get; set; }
        public decimal? CancellationFeePercent { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateDate { get; set; }

        public virtual Restaurant Restaurant { get; set; } = null!;
    }
}
