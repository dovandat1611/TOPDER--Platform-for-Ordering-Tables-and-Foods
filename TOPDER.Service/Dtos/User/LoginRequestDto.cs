using MimeKit.Tnef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.User
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

    }
    public class UserLoginDTO
    {
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }

    }
}
