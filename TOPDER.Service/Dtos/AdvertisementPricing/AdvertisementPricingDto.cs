using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.AdvertisementPricing
{
    public class AdvertisementPricingDto
    {
        public int PricingId { get; set; }
        public int AdminId { get; set; }
        public string PricingName { get; set; } = null!;
        public string? Description { get; set; }
        public int DurationHours { get; set; }
        public decimal Price { get; set; }
    }
}
