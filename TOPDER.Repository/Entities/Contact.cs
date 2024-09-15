using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Contact
    {
        public int ContactId { get; set; }
        public int? Uid { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Topic { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? ContactDate { get; set; }
        public string? Status { get; set; }
        public string Phone { get; set; } = null!;

        public virtual User? UidNavigation { get; set; }
    }
}
