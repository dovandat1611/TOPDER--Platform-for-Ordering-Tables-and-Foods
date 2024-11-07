using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Wallet
{
    public class WalletBankDto
    {
        public int WalletId { get; set; }
        public int Uid { get; set; }
        public string? BankCode { get; set; }
        public string? AccountNo { get; set; }
        public string? AccountName { get; set; }
    }
}
