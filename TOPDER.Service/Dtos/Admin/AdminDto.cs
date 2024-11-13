using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Admin
{
    public class AdminDto
    {
        public int Uid { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime? Dob { get; set; }
        public string? Image { get; set; }
        public string? Role { get; set; }
    }
}
