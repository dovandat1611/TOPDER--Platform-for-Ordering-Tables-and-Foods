using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.VNPAY
{
    public class VNPayModel
    {
        public string BookingDescription { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public string BookingId { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string PaymentId { get; set; } = null!;
        public bool Success { get; set; }
        public string Token { get; set; } = null!;
        public string VnPayResponseCode { get; set; } = null!;
        public string Amount { get; set; } = null!;
        public string PayDate { get; set; } = null!;
    }
}
