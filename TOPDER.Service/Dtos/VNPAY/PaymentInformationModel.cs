using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.VNPAY
{
    public class PaymentInformationModel
    {
        public string BookingID { get; set; } = null!;
        public string AccountID { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public double Amount { get; set; }
        public string PaymentType { get; set; } = string.Empty;
    }
}
