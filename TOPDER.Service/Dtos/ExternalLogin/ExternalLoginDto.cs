using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.ExternalLogin
{
    public class ExternalLoginDto
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public string ExternalProvider { get; set; } = null!;
        public string ExternalUserId { get; set; } = null!;
        public string? AccessToken { get; set; }
    }
}
