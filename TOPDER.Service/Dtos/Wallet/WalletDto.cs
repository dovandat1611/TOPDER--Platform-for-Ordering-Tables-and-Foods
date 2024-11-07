using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Wallet
{
    public class WalletDto
    {
        public int WalletId { get; set; }
        public int Uid { get; set; }
        public decimal? WalletBalance { get; set; }
        public string? BankCode { get; set; }
        public string? AccountNo { get; set; }
        public string? AccountName { get; set; }
        public string? OtpCode { get; set; }
    }
}
