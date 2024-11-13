using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.BookingAdvertisement
{
    public class BookingAdvertisementDto
    {
        public int BookingId { get; set; }
        public int RestaurantId { get; set; }
        public string? Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string? StatusPayment { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
