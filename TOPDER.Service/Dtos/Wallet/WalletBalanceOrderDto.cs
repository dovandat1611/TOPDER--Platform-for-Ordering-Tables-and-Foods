﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Wallet
{
    public class WalletBalanceOrderDto
    {
        public int Uid { get; set; }
        public decimal? WalletBalance { get; set; }
    }
}
