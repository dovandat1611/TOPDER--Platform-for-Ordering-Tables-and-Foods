using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Restaurant
{
    public class UpdateInfoRestaurantDto
    {
        [Required(ErrorMessage = "Uid is required.")]
        public int Uid { get; set; }
        public int? CategoryRestaurantId { get; set; }
        public string NameOwner { get; set; } = null!;
        public string NameRes { get; set; } = null!;
        public string? Logo { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public string? ProvinceCity { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? Subdescription { get; set; }
        public int? TableGapTime { get; set; }
        public decimal? Discount { get; set; }
    }
}
