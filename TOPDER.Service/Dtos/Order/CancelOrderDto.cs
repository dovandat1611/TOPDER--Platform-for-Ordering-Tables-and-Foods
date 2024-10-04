using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Order
{
    public class CancelOrderDto
    {
        public int OrderId { get; set; }
        public int UserCancelID { get; set; }
        public int CustomerID { get; set; }
        public int WalletCustomerId { get; set; }
        public string NameCustomer { get; set; } = string.Empty;
        public string NameRestaurant { get; set; } = string.Empty;
        public string EmailRestaurant { get; set; } = string.Empty;
        public string EmailCustomer { get; set; } = string.Empty;
        public decimal WalletBalanceCustomer { get; set; }
        public decimal? CancellationFeePercent { get; set; }
        public decimal TotalAmount { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
