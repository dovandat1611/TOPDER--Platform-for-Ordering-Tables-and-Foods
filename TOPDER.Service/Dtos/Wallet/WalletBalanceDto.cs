using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Wallet
{
    public class WalletBalanceDto
    {
        public int WalletId { get; set; }
        public int Uid { get; set; }
        public decimal? WalletBalance { get; set; }
        public decimal? TransactionAmount { get; set; }
    }
}
