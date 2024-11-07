using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Feedback
{
    public class FeedbackAdminDto
    {
        public int FeedbackId { get; set; }
        public int? CustomerId { get; set; }
        public int OrderId { get; set; }
        public string? CustomerName { get; set; }
        public int? RestaurantId { get; set; }
        public string? RestaurantName { get; set; }
        public int? Star { get; set; }
        public string? Content { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Status { get; set; }
    }
}
