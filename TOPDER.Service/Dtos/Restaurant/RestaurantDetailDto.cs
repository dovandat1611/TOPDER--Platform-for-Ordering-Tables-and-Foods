using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Discount;
using TOPDER.Service.Dtos.Image;

namespace TOPDER.Service.Dtos.Restaurant
{
    public class RestaurantDetailDto
    {
        public int Uid { get; set; }
        public string? Logo { get; set; }
        public string NameRes { get; set; } = null!;
        public int? CategoryRestaurantId { get; set; }
        public string CategoryName { get; set; } = null!;
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public string Address { get; set; } = null!;
        public decimal? Discount { get; set; }
        public decimal Price { get; set; }
        public decimal MinPriceMenu { get; set; }
        public decimal MaxPriceMenu { get; set; }
        public int TotalFeedbacks { get; set; }
        public int Star { get; set; }
        public decimal? FirstFeePercent { get; set; }
        public decimal? ReturningFeePercent { get; set; }
        public decimal? CancellationFeePercent { get; set; }
        public List<ImageDto> Images { get; set; } = null!;
        public List<RestaurantDto> RelateRestaurant { get; set; } = null!;
    }
}
