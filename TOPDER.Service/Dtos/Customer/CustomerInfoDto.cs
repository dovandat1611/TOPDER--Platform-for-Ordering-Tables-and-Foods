using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Customer
{
    public class CustomerInfoDto
    {
        public int Uid { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Image { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
    }
}
