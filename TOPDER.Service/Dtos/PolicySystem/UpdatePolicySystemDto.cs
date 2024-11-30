using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.PolicySystem
{
    public class UpdatePolicySystemDto
    {
        public int PolicyId { get; set; }
        public decimal MinOrderValue { get; set; }
        public decimal? MaxOrderValue { get; set; }
        public decimal FeeAmount { get; set; }
        public string Status { get; set; } = null!;
    }
}
