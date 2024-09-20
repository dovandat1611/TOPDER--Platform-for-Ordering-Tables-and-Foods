using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class WalletTransaction
    {
        public int TransactionId { get; set; }
        public int WalletId { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionType { get; set; } = null!;
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = null!;

        public virtual Wallet Wallet { get; set; } = null!;
    }
}
