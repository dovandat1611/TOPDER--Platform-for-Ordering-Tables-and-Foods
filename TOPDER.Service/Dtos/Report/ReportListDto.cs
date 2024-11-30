using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Report
{
    public class ReportListDto
    {
        public int ReportId { get; set; }
        public int ReportedBy { get; set; }
        public string ReportedByEmail { get; set; } = string.Empty;
        public int ReportedOn { get; set; }
        public string ReportedOnEmail { get; set; } = string.Empty;
        public string ReportType { get; set; } = null!;
        public int? FeedbackId { get; set; }
        public int? OrderId { get; set; }
        public string Description { get; set; } = null!;
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
