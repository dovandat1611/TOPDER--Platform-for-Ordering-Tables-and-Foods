using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.FeedbackReply;

namespace TOPDER.Service.Dtos.Feedback
{
    public class FeedbackHistoryDto
    {
        public int FeedbackId { get; set; }
        public int? RestaurantId { get; set; }
        public int OrderId { get; set; }
        public string RestaurantName { get; set; } = null!;
        public string? RestaurantImage { get; set; } = null!;
        public int? Star { get; set; }
        public string? Content { get; set; }
        public DateTime CreateDate { get; set; }
        public FeedbackReplyCustomerDto FeedbackReplyCustomer { get; set; } = new FeedbackReplyCustomerDto();
    }
}
