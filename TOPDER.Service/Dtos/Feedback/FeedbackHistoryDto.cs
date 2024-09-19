using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Feedback
{
    public class FeedbackHistoryDto
    {
        public int? RestaurantId { get; set; }
        public string RestaurantName { get; set; } = null!;
        public string? RestaurantImage { get; set; } = null!;
        public int? Star { get; set; }
        public string? Content { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
