using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Report
    {
        public int ReportId { get; set; }
        public int ReportedBy { get; set; }
        public int ReportedOn { get; set; }
        public string ReportType { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual User ReportedByNavigation { get; set; } = null!;
        public virtual User ReportedOnNavigation { get; set; } = null!;
    }
}
