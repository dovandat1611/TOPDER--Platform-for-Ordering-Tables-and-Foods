using System;
using System.Collections.Generic;

namespace TOPDER.Repository.Entities
{
    public partial class Admin
    {
        public Admin()
        {
            Blogs = new HashSet<Blog>();
        }

        public int Uid { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime? Dob { get; set; }
        public string? Image { get; set; }

        public virtual User UidNavigation { get; set; } = null!;
        public virtual ICollection<Blog> Blogs { get; set; }
    }
}
