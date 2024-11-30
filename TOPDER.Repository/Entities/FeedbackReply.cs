using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class FeedbackReply
    {
        public int ReplyId { get; set; }
        public int FeedbackId { get; set; }
        public int RestaurantId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public bool? IsVisible { get; set; }

        public virtual Feedback Feedback { get; set; } = null!;
        public virtual Restaurant Restaurant { get; set; } = null!;
    }
}
