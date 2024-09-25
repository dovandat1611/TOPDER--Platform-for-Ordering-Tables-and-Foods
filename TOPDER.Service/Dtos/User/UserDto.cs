using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.User
{
    public class UserDto
    {
        public int Uid { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public string? OtpCode { get; set; }
        public bool IsVerify { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public bool IsExternalLogin { get; set; }
    }
}
