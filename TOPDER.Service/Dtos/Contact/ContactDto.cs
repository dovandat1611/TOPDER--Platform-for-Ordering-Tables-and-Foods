using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Contact
{
    public class ContactDto
    {
        public int ContactId { get; set; }
        public int? Uid { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Topic { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? ContactDate { get; set; }
        public string Phone { get; set; } = null!;
        public string? Status { get; set; }
    }
}
