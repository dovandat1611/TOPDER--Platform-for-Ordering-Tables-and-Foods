using System;
using System.Collections.Generic;
using TOPDER.Repository.Entities;

namespace TOPDER.Repository.Entities
{
    public partial class AdvertisementPricing
    {
        public int PricingId { get; set; }
        public int AdminId { get; set; }
        public string PricingName { get; set; } = null!;
        public string? Description { get; set; }
        public int DurationHours { get; set; }
        public decimal Price { get; set; }

        public virtual Admin Admin { get; set; } = null!;
    }
}
