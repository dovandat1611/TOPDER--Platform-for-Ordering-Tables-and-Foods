using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Order
{
    public class CompleteOrderDto
    {
        public int OrderId { get; set; }
        public int RestaurantID { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public int WalletId { get; set; }
        public decimal WalletBalance { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
