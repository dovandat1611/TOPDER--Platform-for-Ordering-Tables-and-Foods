using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Feedback
    {
        public Feedback()
        {
            FeedbackReplies = new HashSet<FeedbackReply>();
            Reports = new HashSet<Report>();
        }

        public int FeedbackId { get; set; }
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? RestaurantId { get; set; }
        public int? Star { get; set; }
        public string? Content { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Status { get; set; }
        public bool? IsVisible { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Order Order { get; set; } = null!;
        public virtual Restaurant? Restaurant { get; set; }
        public virtual ICollection<FeedbackReply> FeedbackReplies { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
