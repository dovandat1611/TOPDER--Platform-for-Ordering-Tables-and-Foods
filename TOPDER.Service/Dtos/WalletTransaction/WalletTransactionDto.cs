using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.WalletTransaction
{
    public class WalletTransactionDto
    {
        public int TransactionId { get; set; }
        public int WalletId { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionType { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
    }
}
