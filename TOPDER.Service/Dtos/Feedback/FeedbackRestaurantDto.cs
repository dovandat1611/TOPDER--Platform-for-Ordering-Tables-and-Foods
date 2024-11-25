using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.FeedbackReply;

namespace TOPDER.Service.Dtos.Feedback
{
    public class FeedbackRestaurantDto
    {
        public int FeedbackId { get; set; }
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? Star { get; set; }
        public string? Content { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Status { get; set; }
        public bool isReply { get; set; }
        public FeedbackReplyDto FeedbackReply { get; set; } = new FeedbackReplyDto();
    }
}
