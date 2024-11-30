using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.FeedbackReply
{
    public class CreateFeedbackReplyDto
    {
        public int FeedbackId { get; set; }
        public int RestaurantId { get; set; }
        public string Content { get; set; } = null!;
    }
}
