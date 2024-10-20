using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Feedback
{
    public class FeedbackCustomerDto
    {
        public int FeedbackId { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerImage { get; set; }
        public int? Star { get; set; }
        public string? Content { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
