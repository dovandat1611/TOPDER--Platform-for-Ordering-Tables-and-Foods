﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.User
{
    public class UserOrderIsBalance
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public string Name { get; set; } = null!;
    }
}
