using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.User
{
    public class GetRoleAndBalanceForProfileDto
    {
        public string? Role { get; set; }
        public decimal? WalletBalance { get; set; }
    }
}
