using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.WalletTransaction
{
    public class WalletTransactionWithDrawDto
    {
        public int Uid { get; set; }
        public int WalletId { get; set; }
        public decimal TransactionAmount { get; set; }
    }
}
