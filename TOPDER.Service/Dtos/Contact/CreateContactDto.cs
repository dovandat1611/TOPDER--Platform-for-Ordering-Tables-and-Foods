using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Contact
{
    public class CreateContactDto
    {
        public int? Uid { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Topic { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
