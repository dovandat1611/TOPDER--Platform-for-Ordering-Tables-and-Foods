using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.FeedbackReply
{
    public class FeedbackReplyCustomerDto
    {
        public int ReplyId { get; set; }
        public int FeedbackId { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public string RestaurantImage { get; set; } = string.Empty;
        public string Content { get; set; } = null!;
        public DateTime CreateDate { get; set; }
    }
}
