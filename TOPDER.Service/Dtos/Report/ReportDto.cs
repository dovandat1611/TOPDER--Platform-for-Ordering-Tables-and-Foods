using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Report
{
    public class ReportDto
    {
        public int ReportedBy { get; set; }
        public int ReportedOn { get; set; }
        public int? FeedbackId { get; set; }
        public int? OrderId { get; set; }
        public string ReportType { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
