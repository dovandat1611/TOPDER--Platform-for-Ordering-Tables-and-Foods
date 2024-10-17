using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.User
{
    public class UserLoginDTO
    {
        // CUSTOMER | ADMIN
        public int Uid { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Image { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }

        // RESTAURANT
        public int? CategoryRestaurantId { get; set; }
        public string? NameOwner { get; set; }
        public string? NameRes { get; set; }
        public string? Logo { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? Subdescription { get; set; }
        public string? ProvinceCity { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public decimal? Discount { get; set; }
        public int? MaxCapacity { get; set; }
        public decimal? Price { get; set; }
        public bool? IsBookingEnabled { get; set; }
        public decimal? FirstFeePercent { get; set; }
        public decimal? ReturningFeePercent { get; set; }
        public decimal? CancellationFeePercent { get; set; }

        // ALL 
        public string? Role { get; set; } = string.Empty;
    }
}
