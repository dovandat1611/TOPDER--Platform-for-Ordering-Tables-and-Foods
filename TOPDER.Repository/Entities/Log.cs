using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Log
    {
        public int LogId { get; set; }
        public int RestaurantId { get; set; }
        public string LogType { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual Restaurant Restaurant { get; set; } = null!;
    }
}
