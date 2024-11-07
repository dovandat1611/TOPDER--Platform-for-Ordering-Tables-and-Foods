using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Order
{
    public class CancelOrderRequest
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string? CancelReason { get; set; }
    }
}
