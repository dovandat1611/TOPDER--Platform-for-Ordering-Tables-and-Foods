using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.User
{
    public class CheckValidateUserLoginGG
    {
        public int Uid { get; set; }
        public string Email { get; set; } = null!;
        public bool IsVerify { get; set; }
        public string Status { get; set; } = null!;
        public bool IsExternalLogin { get; set; }
    }
}
