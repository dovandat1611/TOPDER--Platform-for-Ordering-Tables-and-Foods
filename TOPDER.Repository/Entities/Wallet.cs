using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Wallet
    {
        public Wallet()
        {
            WalletTransactions = new HashSet<WalletTransaction>();
        }

        public int WalletId { get; set; }
        public int Uid { get; set; }
        public decimal WalletBalance { get; set; }
        public string BankCode { get; set; } = null!;
        public string AccountNo { get; set; } = null!;
        public string AccountName { get; set; } = null!;
        public decimal Amount { get; set; }
        public string? OtpCode { get; set; }
        public string? Description { get; set; }

        public virtual User UidNavigation { get; set; } = null!;
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
