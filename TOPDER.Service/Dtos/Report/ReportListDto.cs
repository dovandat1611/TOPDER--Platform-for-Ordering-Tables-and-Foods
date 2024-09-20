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
        public int ReportedByEmail { get; set; }
        public int ReportedOn { get; set; }
        public int ReportedOnEmail { get; set; }
        public string ReportType { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
