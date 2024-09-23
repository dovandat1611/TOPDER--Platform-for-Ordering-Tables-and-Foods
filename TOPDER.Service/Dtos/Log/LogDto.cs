using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Log
{
    public class LogDto
    {
        public int LogId { get; set; }
        public int Uid { get; set; }
        public string LogType { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
