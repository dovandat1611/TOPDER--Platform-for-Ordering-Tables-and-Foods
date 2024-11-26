using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.PolicySystem
{
    public class CreatePolicySystemDto
    {
        public int AdminId { get; set; }
        public decimal MinOrderValue { get; set; }
        public decimal? MaxOrderValue { get; set; }
        public decimal FeeAmount { get; set; }
    }
}
