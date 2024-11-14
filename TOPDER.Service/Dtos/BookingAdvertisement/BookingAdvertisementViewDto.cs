using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.BookingAdvertisement
{
    public class BookingAdvertisementViewDto
    {
        public int BookingId { get; set; }
        public int Uid { get; set; }
        public string? Logo { get; set; }
        public string NameRes { get; set; } = null!;
        public string? Title { get; set; }
        public int? CategoryRestaurantId { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
