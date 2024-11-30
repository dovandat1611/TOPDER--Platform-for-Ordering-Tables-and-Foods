using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class PolicySystem
    {
        public int PolicyId { get; set; }
        public int AdminId { get; set; }
        public decimal MinOrderValue { get; set; }
        public decimal? MaxOrderValue { get; set; }
        public decimal FeeAmount { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateDate { get; set; }

        public virtual Admin Admin { get; set; } = null!;
    }
}
