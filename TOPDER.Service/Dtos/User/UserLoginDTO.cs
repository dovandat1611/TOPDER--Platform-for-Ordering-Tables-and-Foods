using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.User
{
    public class UserLoginDTO
    {
        public int Uid { get; set; }
        public string Email { get; set; } = null!;
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
